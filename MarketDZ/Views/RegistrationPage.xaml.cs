// RegistrationPage.xaml.cs
using MarketDZ.ViewModels;
using MarketDZ.Services;
using System.Diagnostics;

namespace MarketDZ.Views
{
    public partial class RegistrationPage : ContentPage
    {
        private readonly RegistrationViewModel _viewModel;

        public RegistrationPage(RegistrationViewModel viewModel)
        {
            Debug.WriteLine("RegistrationPage constructor started");

            try
            {
                Debug.WriteLine("Before InitializeComponent in RegistrationPage");
                InitializeComponent();
                Debug.WriteLine("After InitializeComponent in RegistrationPage");

                Debug.WriteLine($"RegistrationViewModel instance: {(viewModel != null ? "Valid" : "Null")}");
                BindingContext = _viewModel = viewModel;
                Debug.WriteLine("BindingContext set in RegistrationPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in RegistrationPage constructor: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }

            Debug.WriteLine("RegistrationPage constructor completed");
        }

        protected override async void OnAppearing()
        {
            Debug.WriteLine("RegistrationPage OnAppearing started");

            try
            {
                base.OnAppearing();
                Debug.WriteLine("Base OnAppearing called");

                Debug.WriteLine("Before calling InitializeAsync on viewModel");
                await _viewModel.InitializeAsync();
                Debug.WriteLine("After calling InitializeAsync on viewModel");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in RegistrationPage OnAppearing: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Debug.WriteLine("RegistrationPage OnAppearing completed");
        }

        protected override void OnDisappearing()
        {
            Debug.WriteLine("RegistrationPage OnDisappearing started");
            base.OnDisappearing();
            Debug.WriteLine("RegistrationPage OnDisappearing completed");
        }
    }
}