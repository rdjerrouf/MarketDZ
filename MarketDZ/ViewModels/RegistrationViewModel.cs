using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using MarketDZ.Models;
using MarketDZ.Services;
using MarketDZ.Helpers;

namespace MarketDZ.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IVerificationService _verificationService;

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

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public RegistrationViewModel(IAuthService authService, IVerificationService verificationService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        }

        public async Task InitializeAsync()
        {
            // Firebase initialization if needed
            await Task.CompletedTask;
        }

        [RelayCommand(CanExecute = nameof(CanRegister))]
        private async Task Register()
        {
            if (IsBusy) return;

            try
            {
                Debug.WriteLine("\nStarting registration process");
                IsBusy = true;

                // Validation
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    Debug.WriteLine("Validation failed: Empty fields");
                    await ShowError("Please fill in all fields");
                    return;
                }

                if (!InputValidator.IsValidEmail(Email))
                {
                    Debug.WriteLine("Validation failed: Invalid email");
                    await ShowError("Please enter a valid email address");
                    return;
                }

                if (!InputValidator.IsValidPassword(Password))
                {
                    Debug.WriteLine("Validation failed: Invalid password");
                    await ShowError("Password must be at least 8 characters and contain uppercase, lowercase, and numbers");
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    Debug.WriteLine("Validation failed: Passwords don't match");
                    await ShowError("Passwords do not match");
                    return;
                }

                // Check if email is already registered
                bool isEmailRegistered = await _authService.IsEmailRegisteredAsync(Email);
                if (isEmailRegistered)
                {
                    Debug.WriteLine("Email already registered");
                    await ShowError("This email is already registered");
                    return;
                }

                Debug.WriteLine("All validation passed, creating user object");

                // Create user object
                var user = new User
                {
                    Email = Email,
                    PasswordHash = Password, // Will be hashed in the service
                    CreatedAt = DateTime.UtcNow,
                    DisplayName = Email.Split('@')[0], // Set a default display name
                    IsEmailVerified = false,
                    ShowEmail = false,
                    ShowPhoneNumber = false
                };

                Debug.WriteLine("Attempting to register user");
                bool success = await _authService.RegisterUserAsync(user);

                if (success)
                {
                    Debug.WriteLine("Registration successful");

                    // The token generation and verification is handled by FirebaseAuthService
                    await _authService.SendEmailVerificationTokenAsync(user.Id);

                    await ShowMessage("Success", "Registration successful! Please check your email for verification instructions.");
                    await Shell.Current.GoToAsync("SignInPage");
                }
                else
                {
                    Debug.WriteLine("Registration failed: User might already exist");
                    await ShowError("Registration failed. Email might already be in use.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Registration error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                // Show the actual error message
                await ShowError($"Registration failed: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanRegister() => !IsBusy;

        private async Task ShowError(string message)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }
        }

        private async Task ShowMessage(string title, string message)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            }
        }

        [RelayCommand]
        private async Task GoToSignIn()
        {
            await Shell.Current.GoToAsync("SignInPage");
        }
    }
}