using System.Diagnostics;

namespace MarketDZ
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Get the service provider
            var serviceProvider = IPlatformApplication.Current?.Services;

            if (serviceProvider != null)
            {
                // Resolve the FirebaseService
                var firebaseService = serviceProvider.GetService<MarketDZ.Services.FirebaseService>();

                if (firebaseService != null)
                {
                    try
                    {
                        await firebaseService.InitializeAsync();
                        Debug.WriteLine("Firebase initialized successfully");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Firebase initialization error: {ex.Message}");
                    }
                }
            }

            Debug.WriteLine("Application initialization completed");
        }
    }
}
