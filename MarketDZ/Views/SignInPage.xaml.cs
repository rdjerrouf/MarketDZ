// SignInPage.xaml.cs
using MarketDZ.ViewModels;
using System.Diagnostics;
using MarketDZ.Services;

namespace MarketDZ.Views

{
    public partial class SignInPage : ContentPage
    {
        private readonly SignInViewModel _viewModel;

        public SignInPage(SignInViewModel viewModel)
        {
            Debug.WriteLine("SignInPage constructor started");

            try
            {
                Debug.WriteLine("Before InitializeComponent in SignInPage");
                InitializeComponent();
                Debug.WriteLine("After InitializeComponent in SignInPage");

                _viewModel = viewModel;
                Debug.WriteLine($"SignInViewModel instance: {(_viewModel != null ? "Valid" : "Null")}");

                BindingContext = _viewModel;
                Debug.WriteLine("BindingContext set in SignInPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SignInPage constructor: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Re-throw to make sure the error is properly handled
                throw;
            }

            Debug.WriteLine("SignInPage constructor completed");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("SignInPage OnAppearing called");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Debug.WriteLine("SignInPage OnDisappearing called");
        }
    }
}
