﻿// ConfirmEmailViewModel.cs
using MarketDZ.Services;
using MarketDZ.Extensions;
using System.Windows.Input;
using MarketDZ.Views;

namespace MarketDZ.ViewModels
{
    public class ConfirmEmailViewModel : BindableObject
    {
        private readonly IAuthService _authService;

        public ConfirmEmailViewModel(IAuthService authService)
        {
            _authService = authService;
            ConfirmEmailCommand = new Command(async () => await ConfirmEmailAsync());
        }

        public ICommand ConfirmEmailCommand { get; }

        private async Task ConfirmEmailAsync()
        {
            var userId = await Shell.Current.GetQueryParameterAsync("userId");
            var token = await Shell.Current.GetQueryParameterAsync("token");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                await ShowError("Invalid confirmation link");
                return;
            }

            var success = await _authService.ConfirmEmailAsync(userId, token);

            if (success)
            {
                await ShowMessage("Success", "Email confirmed successfully!");
                await Shell.Current.GoToAsync(nameof(SignInPage));
            }
            else
            {
                await ShowError("Email confirmation failed");
            }
        }

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
    }
}