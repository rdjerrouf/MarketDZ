using MarketDZ.ViewModels;
using MarketDZ.Services;

namespace MarketDZ.Views
{
    public partial class StatusManagementPage : ContentPage
    {
        public StatusManagementPage(int itemId)
        {
            InitializeComponent();

            // Get services from dependency injection
            var statusService = Handler.MauiContext.Services.GetService<FirebaseItemStatusService>();
            var firebaseService = Handler.MauiContext.Services.GetService<FirebaseService>();

            // Create view model
            var viewModel = new StatusManagementViewModel(statusService, firebaseService, Navigation);

            // Set binding context
            BindingContext = viewModel;

            // Initialize with item ID
            viewModel.InitializeAsync(itemId);
        }
    }
}