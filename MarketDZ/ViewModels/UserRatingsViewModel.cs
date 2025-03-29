using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketDZ.Models;
using MarketDZ.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MarketDZ.ViewModels
{
    public partial class UserRatingsViewModel : ObservableObject
    {
        private readonly IItemService _itemService;
        private readonly IAuthService _authService;

        private ObservableCollection<Rating> _ratings;
        public ObservableCollection<Rating> Ratings
        {
            get => _ratings;
            set => SetProperty(ref _ratings, value);
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

        private double _averageRating;
        public double AverageRating
        {
            get => _averageRating;
            set => SetProperty(ref _averageRating, value);
        }

        private int _totalRatings;
        public int TotalRatings
        {
            get => _totalRatings;
            set => SetProperty(ref _totalRatings, value);
        }

        private int _userId;
        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        private bool _isCurrentUser;
        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set => SetProperty(ref _isCurrentUser, value);
        }

        private bool _hasRatings;
        public bool HasRatings
        {
            get => _hasRatings;
            set => SetProperty(ref _hasRatings, value);
        }

        public UserRatingsViewModel(IItemService itemService, IAuthService authService)
        {
            _itemService = itemService;
            _authService = authService;
            Ratings = new ObservableCollection<Rating>();
        }

        public async Task InitializeAsync(int userId)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Loading ratings...";
                Ratings.Clear();
                UserId = userId;

                // Check if this is the current user
                var currentUser = await _authService.GetCurrentUserAsync();
                IsCurrentUser = currentUser?.Id == userId;

                // Get user profile information
                var userProfile = await _authService.GetUserProfileAsync(userId);
                if (userProfile != null)
                {
                    DisplayName = userProfile.DisplayName ?? $"User {userId}";
                }

                // Get user statistics including average rating
                var stats = await _itemService.GetUserProfileStatisticsAsync(userId);
                AverageRating = stats.AverageRating;

                // Load user ratings
                var ratings = await _itemService.GetUserRatingsAsync(userId);
                foreach (var rating in ratings)
                {
                    Ratings.Add(rating);
                }

                TotalRatings = Ratings.Count;
                HasRatings = TotalRatings > 0;

                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading ratings: {ex.Message}");
                StatusMessage = "Failed to load ratings.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RefreshRatings()
        {
            await InitializeAsync(UserId);
        }

        [RelayCommand]
        private async Task ViewItem(int itemId)
        {
            await Shell.Current.GoToAsync($"ItemDetailPage?ItemId={itemId}");
        }

        [RelayCommand]
        private async Task MarkHelpful(Rating rating)
        {
            try
            {
                IsBusy = true;

                // This would be implemented in a real app with a method to mark a rating as helpful
                // For now, we'll just increment the count locally
                if (rating.HelpfulVotes.HasValue)
                {
                    rating.HelpfulVotes++;
                }
                else
                {
                    rating.HelpfulVotes = 1;
                }

                // Force a UI update
                var index = Ratings.IndexOf(rating);
                if (index >= 0)
                {
                    Ratings.Remove(rating);
                    Ratings.Insert(index, rating);
                }

                await Task.Delay(500); // Simulate network delay
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error marking helpful: {ex.Message}");
                StatusMessage = "Could not mark rating as helpful.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}