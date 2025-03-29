using Microsoft.Extensions.Logging;
using MarketDZ.Services;
using MarketDZ.ViewModels;
using MarketDZ.Views;
using MarketDZ.Views.AddItem;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;
using MarketDZ.Models;
using MarketDZ.ViewModels.AddItem;

namespace MarketDZ
{
    /// <summary>
    /// Static class responsible for MAUI application setup and configuration.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Initializes the Firebase connection.
        /// </summary>
        /// <param name="app">The MAUI application instance.</param>
        public static async Task InitializeFirebase(MauiApp app)
        {
            Debug.WriteLine("Initializing Firebase...");

            using var scope = app.Services.CreateScope();
            try
            {
                var firebaseService = scope.ServiceProvider.GetRequiredService<FirebaseService>();
                await firebaseService.InitializeAsync();
                Debug.WriteLine("Firebase initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firebase initialization error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Just log the error but allow the app to continue
            }
        }

        /// <summary>
        /// Creates and configures the MAUI app.
        /// </summary>
        /// <returns>The configured MAUI app instance.</returns>
        public static MauiApp CreateMauiApp()
        {
            Debug.WriteLine("Starting application initialization...");

            var builder = MauiApp.CreateBuilder();

            ConfigureBasicSettings(builder);
            ConfigureFirebase(builder);
            RegisterServices(builder);
            ConfigureDebugSettings(builder);

            var app = builder.Build();

            Debug.WriteLine("Application initialization completed");
            return app;
        }

        /// <summary>
        /// Configures basic MAUI application settings.
        /// </summary>
        /// <param name="builder">The MAUI app builder.</param>
        private static void ConfigureBasicSettings(MauiAppBuilder builder)
        {
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    // Add any additional fonts here
                });
        }

        /// <summary>
        /// Configures Firebase settings and connection.
        /// </summary>
        /// <param name="builder">The MAUI app builder.</param>
        private static void ConfigureFirebase(MauiAppBuilder builder)
        {
            string firebaseUrl = "https://marketdz-a6db7-default-rtdb.firebaseio.com/";
            Debug.WriteLine($"Setting up Firebase with URL: {firebaseUrl}");

            builder.Services.AddSingleton<FirebaseService>(provider =>
                new FirebaseService(firebaseUrl));
        }

        /// <summary>
        /// Registers application services with dependency injection.
        /// </summary>
        /// <param name="builder">The MAUI app builder.</param>
        private static void RegisterServices(MauiAppBuilder builder)
        {
            Debug.WriteLine("Registering services...");

            // Core services
            builder.Services.AddSingleton<ISecureStorage>(SecureStorage.Default);

            // Firebase services
            builder.Services.AddScoped<IAuthService, FirebaseAuthService>();
            builder.Services.AddScoped<IItemService, FirebaseItemService>();
            builder.Services.AddSingleton<IUserSessionService, FirebaseUserSessionService>();
            builder.Services.AddSingleton<IMessageService, FirebaseMessageService>();
            builder.Services.AddSingleton<IVerificationService, FirebaseVerificationService>();
            builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
            builder.Services.AddSingleton<IGeolocationService, FirebaseGeolocationService>();
            builder.Services.AddSingleton<IItemLocationService, FirebaseItemLocationService>();
            builder.Services.AddSingleton<IMediaService, FirebaseMediaService>();
            builder.Services.AddSingleton<IEmailService, MockEmailService>();
            builder.Services.AddSingleton<FirebaseItemStatusService>();
            builder.Services.AddSingleton<FirebasePhotoService>();
            builder.Services.AddSingleton<FirebaseService>();

            // Register ViewModels
            builder.Services.AddTransient<RentalItemViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<AddItemViewModel>();
            builder.Services.AddTransient<ForSaleItemViewModel>();
            builder.Services.AddTransient<JobItemViewModel>();
            builder.Services.AddTransient<ServiceItemViewModel>();
            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<SearchViewModel>();
            builder.Services.AddTransient<InboxViewModel>();
            builder.Services.AddTransient<MyListingsViewModel>();
            builder.Services.AddTransient<PasswordResetViewModel>();
            builder.Services.AddTransient<ItemDetailViewModel>();
            builder.Services.AddTransient<MessageDetailViewModel>();
            builder.Services.AddTransient<ItemMapViewModel>();

            builder.Services.AddTransient<RegistrationViewModel>(provider =>
                new RegistrationViewModel(
                    provider.GetRequiredService<IAuthService>(),
                    provider.GetRequiredService<IVerificationService>()
                ));
            // Register Pages
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<SignInPage>();
            builder.Services.AddTransient<AddItemPage>();
            builder.Services.AddTransient<ForSaleItemPage>();
            builder.Services.AddTransient<RentalItemPage>();
            builder.Services.AddTransient<JobItemPage>();
            builder.Services.AddTransient<ServiceItemPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<RegistrationPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<PostItemPage>();
            builder.Services.AddTransient<InboxPage>();
            builder.Services.AddTransient<MyListingsPage>();
            builder.Services.AddTransient<ItemDetailPage>();
            builder.Services.AddTransient<MessageDetailPage>();
            builder.Services.AddTransient<ItemMapPage>();

            Debug.WriteLine("Services registered.");
        }

        /// <summary>
        /// Configures debug settings for development.
        /// </summary>
        /// <param name="builder">The MAUI app builder.</param>
        private static void ConfigureDebugSettings(MauiAppBuilder builder)
        {
#if DEBUG
            builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Debug);
#endif
        }
    }

    // Implementing the IEmailService interface according to your actual interface definition
    public class MockEmailService : IEmailService
    {
        public Task<bool> SendEmailVerificationAsync(string email, string verificationLink)
        {
            Debug.WriteLine($"[MOCK] Email verification sent to {email} with link: {verificationLink}");
            return Task.FromResult(true);
        }

        public Task<bool> SendPasswordResetAsync(string email, string resetLink)
        {
            Debug.WriteLine($"[MOCK] Password reset sent to {email} with link: {resetLink}");
            return Task.FromResult(true);
        }

        public Task<bool> SendGenericEmailAsync(string email, string subject, string body)
        {
            Debug.WriteLine($"[MOCK] Generic email sent to {email} with subject: {subject}");
            return Task.FromResult(true);
        }

        public Task<bool> SendWelcomeEmailAsync(string email, string userName)
        {
            Debug.WriteLine($"[MOCK] Welcome email sent to {email} for user: {userName}");
            return Task.FromResult(true);
        }
    }
}