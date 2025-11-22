using Microsoft.AspNetCore.Identity;
using Electro.Core.Models.Identity;
using Electro.Reposatory.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

// سكريبت لإنشاء حساب أدمن
// استخدم هذا الكود في Program.cs أو Migration

public static class AdminUserSeeder
{
    public static async Task SeedAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<AppIdentityDbContext>();

        // 1. إنشاء Role "Admin" لو مش موجود
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            Console.WriteLine("✅ Role 'Admin' تم إنشاؤه");
        }
        else
        {
            Console.WriteLine("ℹ️ Role 'Admin' موجود بالفعل");
        }

        // 2. التحقق من وجود المستخدم
        var adminEmail = "admin@electro.com";
        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (existingUser != null)
        {
            Console.WriteLine($"⚠️ المستخدم {adminEmail} موجود بالفعل");
            
            // التحقق من Role
            var roles = await userManager.GetRolesAsync(existingUser);
            if (!roles.Contains("Admin"))
            {
                await userManager.AddToRoleAsync(existingUser, "Admin");
                Console.WriteLine("✅ تم إضافة Role 'Admin' للمستخدم الموجود");
            }
            else
            {
                Console.WriteLine("✅ المستخدم لديه Role 'Admin' بالفعل");
            }
            return;
        }

        // 3. إنشاء المستخدم الجديد
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
            Console.WriteLine($"✅ تم إنشاء حساب الأدمن بنجاح!");
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
}

