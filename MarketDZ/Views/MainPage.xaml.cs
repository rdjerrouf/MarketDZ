using MarketDZ.ViewModels;
using System.Diagnostics;
using MarketDZ.Services;


namespace MarketDZ.Views
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initialize MainPage with proper dependency injection
        /// </summary>
        // MainPage.xaml.cs
        public MainPage(MainViewModel viewModel)
        {
            Debug.WriteLine("MainPage constructor started");

            try
            {
                Debug.WriteLine("Before InitializeComponent in MainPage");
                InitializeComponent();
                Debug.WriteLine("After InitializeComponent in MainPage");

                Debug.WriteLine($"MainViewModel instance: {(viewModel != null ? "Valid" : "Null")}");
                BindingContext = viewModel;
                Debug.WriteLine("BindingContext set in MainPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in MainPage constructor: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }

            Debug.WriteLine("MainPage constructor completed");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("MainPage OnAppearing called");
        }
    }
}