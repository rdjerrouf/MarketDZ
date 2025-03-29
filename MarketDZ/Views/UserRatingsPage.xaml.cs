using MarketDZ.Extensions;
using MarketDZ.ViewModels;
using System.Web;

namespace MarketDZ.Views
{
    public partial class UserRatingsPage : ContentPage
    {
        private readonly UserRatingsViewModel _viewModel;

        public UserRatingsPage(UserRatingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Get the user ID from query parameter
                var query = HttpUtility.ParseQueryString(Shell.Current.CurrentState.Location.ToString());
                var userIdString = query["UserId"];

                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                {
                    await _viewModel.InitializeAsync(userId);
                }
                else
                {
                    await DisplayAlert("Error", "Invalid user ID", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}
