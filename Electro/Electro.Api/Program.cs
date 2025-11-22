using System.Text.Json.Serialization;
using Electro.Apis.Extentions;
using Electro.Core.Errors;
using Electro.Core.Interface;
using Electro.Core.Models.Identity;
using Electro.Reposatory.Data.Identity;
using Electro.Service;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
try
{
    string pathToCredentials = Path.Combine(Environment.CurrentDirectory, "wwwroot", "elctro-ed5d4-firebase-adminsdk-fbsvc-1680ef9784.json");
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(pathToCredentials),
    });
}
catch (Exception ex)
{
    Console.WriteLine("Error initializing Firebase: " + ex.Message);
    throw; // لإظهار تفاصيل الخطأ في سجلات التطبيق
}
// ===== Controllers & JSON =====
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// ===== Response Caching =====
builder.Services.AddResponseCaching();

// ===== Identity + Auth + DB + App Services (كل التسجيلات الأساسية هنا) =====
builder.Services.AddIdentityServices(builder.Configuration);

// ===== CORS =====
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontCors", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ===== SignalR =====
builder.Services.AddSignalR()
    .AddJsonProtocol(o => o.PayloadSerializerOptions.PropertyNamingPolicy = null);

// ===== Swagger + بقية خدماتك =====
builder.Services.AddSwaggerService();
builder.Services.AddAplictionService();
builder.Services.AddMemoryCache();

// ===== لا تسجّل IFirebaseProvider/INotificationService هنا (مسجلين في AddIdentityServices) =====
// builder.Services.AddSingleton<IFirebaseProvider, FirebaseProvider>();
// builder.Services.AddScoped<INotificationService, NotificationService>();

// ===== دعم JWT في SignalR (لو مش متضاف في AddIdentityServices) =====
builder.Services.PostConfigureAll<JwtBearerOptions>(opts =>
{
    opts.Events ??= new JwtBearerEvents();
    var prev = opts.Events.OnMessageReceived;
    opts.Events.OnMessageReceived = ctx =>
    {
        // السماح للتوكن في QueryString عند مسارات SignalR (لو بتستخدم /hubs)
        var accessToken = ctx.Request.Query["access_token"];
        var path = ctx.HttpContext.Request.Path;
        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            ctx.Token = accessToken;
        return prev?.Invoke(ctx) ?? Task.CompletedTask;
    };
});

var app = builder.Build();

// ===== Errors =====
app.UseStatusCodePagesWithReExecute("/errors/{0}");
app.UseMiddleware<ExeptionMiddleWares>();

app.UseStaticFiles();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseRouting();

app.UseResponseCaching();

app.UseCors("FrontCors");

app.UseAuthentication();   // ✅ قبل Authorization
app.UseAuthorization();

// ===== SignalR Hubs =====
app.MapHub<ChatHub>("/ChatHub"); // لاحظ السلاش

// ===== REST Controllers =====
app.MapControllers();

// ===== تشغيل الفرونت إند تلقائياً =====
if (app.Environment.IsDevelopment())
{
    Task.Run(async () =>
    {
        await Task.Delay(2000); // انتظار حتى يبدأ الباك إند
        
        try
        {
            // التحقق من أن الفرونت إند غير شغال
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            var response = await client.GetAsync("http://localhost:3000");
            // إذا وصل هنا، الفرونت إند شغال بالفعل
            Console.WriteLine("ℹ️ الفرونت إند شغال بالفعل");
        }
        catch
        {
            // الفرونت إند غير شغال، شغّله
            var currentDir = Directory.GetCurrentDirectory();
            var solutionDir = Directory.GetParent(currentDir)?.Parent?.FullName ?? currentDir;
            var frontendPath = Path.Combine(solutionDir, "electro-frontend");
            
            if (Directory.Exists(frontendPath))
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c cd /d \"{frontendPath}\" && npm run dev",
                        WindowStyle = ProcessWindowStyle.Minimized,
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };
                    Process.Start(startInfo);
                    await Task.Delay(1000);
                    Console.WriteLine("✅ تم تشغيل الفرونت إند تلقائياً على http://localhost:3000");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ فشل تشغيل الفرونت إند: {ex.Message}");
                    Console.WriteLine($"   تأكد من تشغيله يدوياً: cd {frontendPath} && npm run dev");
                }
            }
            else
            {
                Console.WriteLine($"⚠️ مجلد الفرونت إند غير موجود: {frontendPath}");
            }
        }
    });
}

// ===== Seed Admin User =====
using (var scope = app.Services.CreateScope())
{
    try
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // إنشاء Role "Admin" لو مش موجود
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            Console.WriteLine("✅ Role 'Admin' تم إنشاؤه");
        }

        // التحقق من وجود المستخدم
        var adminEmail = "admin@electro.com";
        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (existingUser == null)
        {
            // إنشاء المستخدم الجديد
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Admin User",
                PhoneNumber = "01234567890",
                Role = "Admin",
                Status = UserStatus.Active
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("✅ تم إنشاء حساب الأدمن بنجاح!");
                Console.WriteLine($"📧 Email: {adminEmail}");
                Console.WriteLine($"🔑 Password: Admin123");
            }
            else
            {
                Console.WriteLine("❌ فشل إنشاء حساب الأدمن:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"   - {error.Description}");
                }
            }
        }
        else
        {
            // التحقق من Role
            var roles = await userManager.GetRolesAsync(existingUser);
            if (!roles.Contains("Admin"))
            {
                await userManager.AddToRoleAsync(existingUser, "Admin");
                Console.WriteLine("✅ تم إضافة Role 'Admin' للمستخدم الموجود");
            }
            else
            {
                Console.WriteLine($"ℹ️ حساب الأدمن موجود بالفعل: {adminEmail}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ خطأ في إنشاء حساب الأدمن: {ex.Message}");
    }
}

app.Run();
