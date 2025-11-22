// File: Services/FirebaseProvider.cs
using Electro.Core.Interface;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

public class FirebaseProvider : IFirebaseProvider
{
    private readonly ILogger<FirebaseProvider> _logger;
    public FirebaseMessaging? CustomerMessaging { get; private set; }
    public bool IsReady { get; private set; }

    public FirebaseProvider(IWebHostEnvironment env, ILogger<FirebaseProvider> logger)
    {
        _logger = logger;

        try
        {
            // مسار الملف مباشرة جوه wwwroot
            var path = Path.Combine(env.ContentRootPath, "wwwroot", "elctro-ed5d4-firebase-adminsdk-fbsvc-1680ef9784.json");

            if (!File.Exists(path))
            {
                _logger.LogWarning("Firebase credentials not found. Path={Path}. FCM disabled.", path);
                IsReady = false;
                return;
            }

            FirebaseApp app;
            try
            {
                // جرّب تجيب الـ app لو متسجل بالفعل
                app = FirebaseApp.GetInstance("ElectroFirebaseApp");
            }
            catch (InvalidOperationException)
            {
                // لو مش موجود → اعمله create
                app = FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(path)
                }, "ElectroFirebaseApp");
            }

            CustomerMessaging = FirebaseMessaging.GetMessaging(app);
            IsReady = true;
            _logger.LogInformation("Firebase initialized successfully. Path={Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase. FCM disabled.");
            IsReady = false;
        }
    }

    public FirebaseMessaging? GetByRole(string role) => CustomerMessaging;
}
