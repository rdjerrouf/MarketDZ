using System;
using System.Windows.Input;
using MarketDZ.Models;
using MarketDZ.Services;
using MarketDZ.Views;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MarketDZ.ViewModels
{
    public class StatusManagementViewModel : ObservableObject
    {
        private readonly FirebaseItemStatusService _statusService;
        private readonly FirebaseService _firebaseService;
        private readonly INavigation _navigation;

        private int _itemId;
        private string _itemTitle = string.Empty;
        private string _statusText = string.Empty;
        private string _statusColor = string.Empty;
        private string _photoUrl = string.Empty;
        private bool _hasPhoto;
        private bool _isBusy;
        private ItemStatus _currentStatus;

        public int ItemId
        {
            get => _itemId;
            set => SetProperty(ref _itemId, value);
        }

        public string ItemTitle
        {
            get => _itemTitle;
            set => SetProperty(ref _itemTitle, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public string StatusColor
        {
            get => _statusColor;
            set => SetProperty(ref _statusColor, value);
        }

        public string PhotoUrl
        {
            get => _photoUrl;
            set => SetProperty(ref _photoUrl, value);
        }

        public bool HasPhoto
        {
            get => _hasPhoto;
            set => SetProperty(ref _hasPhoto, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        // Status change enablement properties
        public bool CanMarkAsActive => _currentStatus != ItemStatus.Active;
        public bool CanMarkAsSold => _currentStatus != ItemStatus.Sold;
        public bool CanMarkAsRented => _currentStatus != ItemStatus.Rented;
        public bool CanMarkAsUnavailable => _currentStatus != ItemStatus.Unavailable;

        // Commands
        public ICommand MarkAsActiveCommand { get; }
        public ICommand MarkAsSoldCommand { get; }
        public ICommand MarkAsRentedCommand { get; }
        public ICommand MarkAsUnavailableCommand { get; }
        public ICommand ManagePhotosCommand { get; }
        public ICommand GoBackCommand { get; }

        public StatusManagementViewModel(FirebaseItemStatusService statusService, FirebaseService firebaseService, INavigation navigation)
        {
            _statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            // Initialize commands
            MarkAsActiveCommand = new Command(async () => await ExecuteMarkAsActiveCommand(), () => CanMarkAsActive);
            MarkAsSoldCommand = new Command(async () => await ExecuteMarkAsSoldCommand(), () => CanMarkAsSold);
            MarkAsRentedCommand = new Command(async () => await ExecuteMarkAsRentedCommand(), () => CanMarkAsRented);
            MarkAsUnavailableCommand = new Command(async () => await ExecuteMarkAsUnavailableCommand(), () => CanMarkAsUnavailable);
            ManagePhotosCommand = new Command(async () => await ExecuteManagePhotosCommand());
            GoBackCommand = new Command(async () => await _navigation.PopAsync());
        }

        public async Task InitializeAsync(int itemId)
        {
            ItemId = itemId;
            await LoadItemDataAsync();
        }

        private async Task LoadItemDataAsync()
        {
            if (ItemId <= 0)
                return;

            try
            {
                IsBusy = true;

                // Get item using Firebase instead of Entity Framework
                var item = await _firebaseService.GetItemByIdAsync(ItemId);

                if (item != null)
                {
                    ItemTitle = item.Title;
                    _currentStatus = item.Status;
                    UpdateStatusUI(item.Status);

                    // Get photos from Firebase
                    // Note that with Firebase, we might need to fetch photos separately
                    // depending on how they're stored
                    if (item.Photos != null && item.Photos.Any())
                    {
                        var primaryPhoto = item.Photos.FirstOrDefault(p => p.IsPrimaryPhoto);
                        if (primaryPhoto != null)
                        {
                            PhotoUrl = primaryPhoto.PhotoUrl;
                            HasPhoto = true;
                        }
                        else if (!string.IsNullOrEmpty(item.PhotoUrl))
                        {
                            PhotoUrl = item.PhotoUrl;
                            HasPhoto = true;
                        }
                        else
                        {
                            HasPhoto = false;
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.PhotoUrl))
                    {
                        PhotoUrl = item.PhotoUrl;
                        HasPhoto = true;
                    }
                    else
                    {
                        HasPhoto = false;
                    }

                    // Update command can execute states
                    ((Command)MarkAsActiveCommand).ChangeCanExecute();
                    ((Command)MarkAsSoldCommand).ChangeCanExecute();
                    ((Command)MarkAsRentedCommand).ChangeCanExecute();
                    ((Command)MarkAsUnavailableCommand).ChangeCanExecute();
                }
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load item: {ex.Message}", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void UpdateStatusUI(ItemStatus status)
        {
            StatusText = status.ToString();

            StatusColor = status switch
            {
                ItemStatus.Active => "#28a745",      // Green
                ItemStatus.Sold => "#dc3545",        // Red
                ItemStatus.Rented => "#fd7e14",      // Orange
                ItemStatus.Unavailable => "#6c757d", // Gray
                _ => "#28a745"                       // Default Green
            };
        }

        private async Task ExecuteMarkAsActiveCommand()
        {
            await ChangeItemStatusAsync(ItemStatus.Active);
        }

        private async Task ExecuteMarkAsSoldCommand()
        {
            await ChangeItemStatusAsync(ItemStatus.Sold);
        }

        private async Task ExecuteMarkAsRentedCommand()
        {
            await ChangeItemStatusAsync(ItemStatus.Rented);
        }

        private async Task ExecuteMarkAsUnavailableCommand()
        {
            await ChangeItemStatusAsync(ItemStatus.Unavailable);
        }

        private async Task ChangeItemStatusAsync(ItemStatus newStatus)
        {
            try
            {
                IsBusy = true;

                bool success = false;

                switch (newStatus)
                {
                    case ItemStatus.Active:
                        success = await _statusService.MarkAsActiveAsync(ItemId);
                        break;
                    case ItemStatus.Sold:
                        success = await _statusService.MarkAsSoldAsync(ItemId);
                        break;
                    case ItemStatus.Rented:
                        success = await _statusService.MarkAsRentedAsync(ItemId);
                        break;
                    case ItemStatus.Unavailable:
                        success = await _statusService.MarkAsUnavailableAsync(ItemId);
                        break;
                }

                if (success)
                {
                    _currentStatus = newStatus;
                    UpdateStatusUI(newStatus);

                    // Update commands
                    ((Command)MarkAsActiveCommand).ChangeCanExecute();
                    ((Command)MarkAsSoldCommand).ChangeCanExecute();
                    ((Command)MarkAsRentedCommand).ChangeCanExecute();
                    ((Command)MarkAsUnavailableCommand).ChangeCanExecute();

                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Success",
                            $"Item marked as {newStatus}",
                            "OK");
                    }
                }
                else
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Error",
                            $"Failed to mark item as {newStatus}",
                            "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        $"Failed to update status: {ex.Message}",
                        "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteManagePhotosCommand()
        {
            // Navigate to photo management page
            var photoManagementPage = new PhotoManagementPage(ItemId);
            await _navigation.PushAsync(photoManagementPage);
        }
    }
}