using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using MarketDZ.Models;
using MarketDZ.Services;

namespace MarketDZ.ViewModels
{
    public partial class SignInViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IUserSessionService _sessionService;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isSigningIn;
        public bool IsSigningIn
        {
            get => _isSigningIn;
            set => SetProperty(ref _isSigningIn, value);
        }

        public SignInViewModel(IAuthService authService, IUserSessionService sessionService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        [RelayCommand]
        private async Task SignIn()
        {
            if (IsSigningIn) return;

            try
            {
                IsSigningIn = true;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                    return;
                }

                Debug.WriteLine($"Attempting sign in for email: {Email}");

                var user = await _authService.SignInAsync(Email, Password);

                if (user is not null)
                {
                    // Store the user in the session service
                    _sessionService.SetCurrentUser(user);
                    await _sessionService.SaveSessionAsync();

                    Debug.WriteLine("Sign in successful, navigating to MainPage");
                    await Shell.Current.GoToAsync("/");
                }
                else
                {
                    Debug.WriteLine("Sign in failed - invalid credentials");
                    await Shell.Current.DisplayAlert("Error", "Invalid email or password", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sign in error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                var errorMessage = "An error occurred during sign in. ";
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    errorMessage += ex.InnerException.Message;
                }
                else
                {
                    errorMessage += ex.Message;
                }

                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
            }
            finally
            {
                IsSigningIn = false;
            }
        }

        [RelayCommand]
        private async Task Register()
        {
            Debug.WriteLine("Register command invoked");
            if (IsSigningIn) return;
            await Shell.Current.GoToAsync("RegistrationPage");
        }
    }
}