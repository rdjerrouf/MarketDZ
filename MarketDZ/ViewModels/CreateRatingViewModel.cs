using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketDZ.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MarketDZ.ViewModels
{
    public partial class CreateRatingViewModel : ObservableObject
    {
        private readonly IItemService _itemService;
        private readonly IAuthService _authService;

        private int _rating;
        public int Rating
        {
            get => _rating;
            set => SetProperty(ref _rating, value);
        }

        private string _review = string.Empty;
        public string Review
        {
            get => _review;
            set => SetProperty(ref _review, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private int _itemId;
        public int ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }

        private int _sellerId;
        public int SellerId
        {
            get => _sellerId;
            set => SetProperty(ref _sellerId, value);
        }

        private string _itemTitle = string.Empty;
        public string ItemTitle
        {
            get => _itemTitle;
            set => SetProperty(ref _itemTitle, value);
        }

        private string _sellerName = string.Empty;
        public string SellerName
        {
            get => _sellerName;
            set => SetProperty(ref _sellerName, value);
        }

        public CreateRatingViewModel(IItemService itemService, IAuthService authService)
        {
            _itemService = itemService;
            _authService = authService;
            Rating = 5; // Default to 5 stars
            Review = string.Empty;
        }

        public async Task InitializeAsync(int itemId, int sellerId)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading...";

                ItemId = itemId;
                SellerId = sellerId;

                // Get item details
                var item = await _itemService.GetItemAsync(itemId);
                if (item != null)
                {
                    ItemTitle = item.Title;
                }

                // Get seller details
                var seller = await _authService.GetUserProfileAsync(sellerId);
                if (seller != null)
                {
                    SellerName = seller.DisplayName ?? $"User {sellerId}";
                }

                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing rating: {ex.Message}");
                StatusMessage = "Failed to load details.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SubmitRating()
        {
            if (Rating < 1 || Rating > 5)
            {
                StatusMessage = "Please select a rating between 1 and 5 stars.";
                return;
            }

            try
            {
                IsBusy = true;
                StatusMessage = "Submitting rating...";

                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    StatusMessage = "You must be logged in to submit a rating.";
                    return;
                }

                bool success = await _itemService.AddRatingAsync(
                    currentUser.Id,
                    ItemId,
                    Rating,
                    Review);

                if (success)
                {
                    StatusMessage = "Rating submitted successfully!";
                    await Shell.Current.DisplayAlert("Success", "Your rating has been submitted.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    StatusMessage = "Failed to submit rating. Please try again.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error submitting rating: {ex.Message}");
                StatusMessage = "An error occurred while submitting the rating.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void SetRating(int value)
        {
            if (value >= 1 && value <= 5)
            {
                Rating = value;
            }
        }
    }
}