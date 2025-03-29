using System.Collections.ObjectModel;
using System.Windows.Input;
using MarketDZ.Models;
using MarketDZ.Services;
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.CompilerServices;

namespace MarketDZ.ViewModels
{
    public partial class PhotoManagementViewModel : INotifyPropertyChanged
    {
        private readonly FirebasePhotoService _photoService;
        private readonly FirebaseService _firebaseService;
        private readonly INavigation _navigation;
        private readonly ObservableCollection<ItemPhoto> _photos;
        private int _itemId;

        private string _itemTitle;
        public string ItemTitle
        {
            get => _itemTitle;
            set => SetProperty(ref _itemTitle, value);
        }

        private string _itemStatus;
        public string ItemStatus
        {
            get => _itemStatus;
            set => SetProperty(ref _itemStatus, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public ObservableCollection<ItemPhoto> Photos => _photos;

        public int ItemId
        {
            get => _itemId;
            private set => SetProperty(ref _itemId, value);
        }

        public PhotoManagementViewModel(FirebasePhotoService photoService, FirebaseService firebaseService, INavigation navigation)
        {
            _photoService = photoService ?? throw new ArgumentNullException(nameof(photoService));
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            _photos = new ObservableCollection<ItemPhoto>();
        }

        public async Task InitializeAsync(int itemId)
        {
            ItemId = itemId;
            await LoadItemDetailsAsync();
            await RefreshPhotos();
        }

        private async Task LoadItemDetailsAsync()
        {
            try
            {
                Debug.WriteLine($"Loading item details for ItemId: {ItemId}");
                var item = await _firebaseService.GetItemByIdAsync(ItemId);
                if (item != null)
                {
                    ItemTitle = item.Title;
                    ItemStatus = item.Status.ToString();
                    Debug.WriteLine($"Item details loaded: {ItemTitle}, Status: {ItemStatus}");
                }
                else
                {
                    Debug.WriteLine($"Item with ID {ItemId} not found");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading item details: {ex.Message}");
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load item details: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task RefreshPhotos()
        {
            if (ItemId <= 0)
            {
                Debug.WriteLine("Invalid ItemId, cannot refresh photos");
                return;
            }

            try
            {
                IsRefreshing = true;
                IsBusy = true;
                Debug.WriteLine($"Refreshing photos for ItemId: {ItemId}");

                var photos = await _photoService.GetItemPhotosAsync(ItemId);
                Debug.WriteLine($"Retrieved {photos.Count} photos");

                _photos.Clear();
                foreach (var photo in photos)
                {
                    _photos.Add(photo);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing photos: {ex.Message}");
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load photos: {ex.Message}", "OK");
                }
            }
            finally
            {
                IsRefreshing = false;
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddPhoto()
        {
            try
            {
                Debug.WriteLine("Initiating photo selection...");
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select a photo"
                });

                if (result != null)
                {
                    Debug.WriteLine($"Photo selected: {result.FileName}");
                    IsBusy = true;

                    var photo = await _photoService.AddPhotoAsync(ItemId, result);
                    Debug.WriteLine($"Photo added successfully with ID: {photo.Id}");
                    _photos.Add(photo);

                    await Application.Current.MainPage.DisplayAlert("Success", "Photo added successfully", "OK");
                }
                else
                {
                    Debug.WriteLine("No photo was selected");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding photo: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to add photo: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task HandlePhotoTapped(ItemPhoto photo)
        {
            if (photo == null)
            {
                Debug.WriteLine("HandlePhotoTapped called with null photo");
                return;
            }

            Debug.WriteLine($"Photo tapped: ID={photo.Id}, IsPrimary={photo.IsPrimaryPhoto}");
            string action = string.Empty;
            if (Application.Current?.MainPage != null)
            {
                action = await Application.Current.MainPage.DisplayActionSheet(
                    "Photo Options",
                    "Cancel",
                    null,
                    photo.IsPrimaryPhoto ? "Delete" : "Set as Primary",
                    photo.IsPrimaryPhoto ? null : "Delete");
            }

            Debug.WriteLine($"Action selected: {action}");

            try
            {
                IsBusy = true;

                if (action == "Set as Primary")
                {
                    Debug.WriteLine($"Setting photo {photo.Id} as primary");
                    await _photoService.SetPrimaryPhotoAsync(photo.Id);
                    await RefreshPhotos();
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Success", "Primary photo updated", "OK");
                    }
                }
                else if (action == "Delete")
                {
                    bool confirm = false;
                    if (Application.Current?.MainPage != null)
                    {
                        confirm = await Application.Current.MainPage.DisplayAlert(
                            "Confirm Delete",
                            "Are you sure you want to delete this photo?",
                            "Yes", "No");
                    }

                    if (confirm)
                    {
                        Debug.WriteLine($"Deleting photo {photo.Id}");
                        await _photoService.DeletePhotoAsync(photo.Id);
                        await RefreshPhotos();
                        if (Application.Current?.MainPage != null)
                        {
                            await Application.Current.MainPage.DisplayAlert("Success", "Photo deleted", "OK");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Photo deletion cancelled by user");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing photo action: {ex.Message}");
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to process request: {ex.Message}", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoBack()
        {
            Debug.WriteLine("Navigating back...");
            await _navigation.PopAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}