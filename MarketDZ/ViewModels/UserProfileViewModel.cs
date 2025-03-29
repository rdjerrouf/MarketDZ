using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketDZ.Models;
using MarketDZ.Models.Dtos;
using MarketDZ.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MarketDZ.ViewModels
{
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class UserProfileViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IItemService _itemService;
        private readonly FirebaseSecurityService _securityService;

        private int _userId;
        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private UserProfileDto _profile;
        public UserProfileDto Profile
        {
            get => _profile;
            set => SetProperty(ref _profile, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isCurrentUser;
        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set => SetProperty(ref _isCurrentUser, value);
        }

        private int _postedItemsCount;
        public int PostedItemsCount
        {
            get => _postedItemsCount;
            set => SetProperty(ref _postedItemsCount, value);
        }

        private double _averageRating;
        public double AverageRating
        {
            get => _averageRating;
            set => SetProperty(ref _averageRating, value);
        }

        private bool _isUserBlocked;
        public bool IsUserBlocked
        {
            get => _isUserBlocked;
            set => SetProperty(ref _isUserBlocked, value);
        }

        public UserProfileViewModel(IAuthService authService, IItemService itemService, FirebaseSecurityService securityService)
        {
            _authService = authService;
            _itemService = itemService;
            _securityService = securityService;
            Profile = new UserProfileDto();
            StatusMessage = string.Empty;
            _profile = new UserProfileDto();
            _statusMessage = string.Empty;
        }
        public async Task InitializeAsync()
        {
            if (UserId <= 0)
                return;

            try
            {
                IsBusy = true;

                // Get current user to check if this is the current user's profile
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    IsCurrentUser = UserId == currentUser.Id;

                    // Check if this user is blocked
                    IsUserBlocked = await _securityService.IsUserBlockedAsync(currentUser.Id, UserId);
                }

                // Load profile
                await LoadProfileAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing profile: {ex.Message}");
                StatusMessage = "Failed to load profile";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadProfileAsync()
        {
            try
            {
                var userProfile = await _authService.GetUserProfileAsync(UserId);
                if (userProfile != null)
                {
                    Profile = userProfile;

                    // Load statistics
                    var stats = await _itemService.GetUserProfileStatisticsAsync(UserId);
                    PostedItemsCount = stats.PostedItemsCount;
                    AverageRating = stats.AverageRating;
                }
                else
                {
                    StatusMessage = "Could not load profile";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading profile: {ex.Message}");
                StatusMessage = "Failed to load profile data";
            }
        }

        [RelayCommand]
        private async Task ViewUserItems()
        {
            await Shell.Current.GoToAsync($"//UserItemsPage?UserId={UserId}");
        }

        [RelayCommand]
        private async Task ViewUserRatings()
        {
            await Shell.Current.GoToAsync($"UserRatingsPage?UserId={UserId}");
        }

        [RelayCommand]
        private async Task BlockUser()
        {
            if (IsCurrentUser)
                return; // Can't block yourself

            bool confirm = await Shell.Current.DisplayAlert(
                "Block User",
                $"Are you sure you want to block {Profile.DisplayName ?? Profile.Email}? They won't be able to see your listings or contact you.",
                "Block", "Cancel");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;

                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                    return;

                await _securityService.BlockUserAsync(
                    currentUser.Id,
                    UserId,
                    "Blocked by user");
                await Shell.Current.DisplayAlert(
                    "User Blocked",
                    $"{Profile.DisplayName ?? Profile.Email} has been blocked",
                    "OK");

                IsUserBlocked = true;

                // Optionally navigate back
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error blocking user: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to block user", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UnblockUser()
        {
            if (!IsUserBlocked)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Unblock User",
                $"Are you sure you want to unblock {Profile.DisplayName ?? Profile.Email}?",
                "Unblock", "Cancel");

            if (!confirm)
                return;

            try
            {
                IsBusy = true;

                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                    return;

                bool success = await _securityService.UnblockUserAsync(
                    currentUser.Id,
                    UserId);

                if (success)
                {
                    await Shell.Current.DisplayAlert(
                        "User Unblocked",
                        $"{Profile.DisplayName ?? Profile.Email} has been unblocked",
                        "OK");

                    IsUserBlocked = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error unblocking user: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to unblock user", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}