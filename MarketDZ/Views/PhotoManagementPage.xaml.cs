using MarketDZ.Services;
using MarketDZ.ViewModels;
using MarketDZ.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MarketDZ.Views
{
    public partial class PhotoManagementPage : ContentPage
    {
        private readonly PhotoManagementViewModel? _viewModel;

        public PhotoManagementPage(int itemId)
        {
            InitializeComponent();

            // Get services from DI container
            var mauiContext = Application.Current?.Handler?.MauiContext;
            if (mauiContext == null)
            {
                throw new ArgumentNullException(nameof(mauiContext), "MauiContext cannot be null");
            }

            // Get Firebase services
            var firebasePhotoService = mauiContext.Services.GetService<FirebasePhotoService>();
            var firebaseService = mauiContext.Services.GetService<FirebaseService>();

            if (firebasePhotoService == null || firebaseService == null)
            {
                throw new ArgumentNullException($"{nameof(firebasePhotoService)} or {nameof(firebaseService)} cannot be null");
            }

            // Create and initialize the view model
            _viewModel = new PhotoManagementViewModel(firebasePhotoService, firebaseService, Navigation);
            BindingContext = _viewModel;

            // Initialize the view model with item ID
            Loaded += async (s, e) =>
            {
                await _viewModel.InitializeAsync(itemId);
            };
        }
    }
}