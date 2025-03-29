using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketDZ.Models;
using MarketDZ.Models.Dtos;
using MarketDZ.Services;
using MarketDZ.Views;
using Microsoft.Maui.Devices.Sensors;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MarketDZ.ViewModels
{
    public partial class NearbyItemsViewModel : ObservableObject
    {
        private readonly IItemService _itemService;
        private readonly IItemLocationService _itemLocationService;
        private readonly IGeolocationService _geolocationService;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private double _searchRadius = 10; // Default radius in km
        public double SearchRadius
        {
            get => _searchRadius;
            set => SetProperty(ref _searchRadius, value);
        }

        private string _locationName;
        public string LocationName
        {
            get => _locationName;
            set => SetProperty(ref _locationName, value);
        }

        private bool _hasLocation;
        public bool HasLocation
        {
            get => _hasLocation;
            set => SetProperty(ref _hasLocation, value);
        }

        private Location? _currentLocation;

        public NearbyItemsViewModel(
            IItemService itemService,
            IItemLocationService itemLocationService,
            IGeolocationService geolocationService)
        {
            _itemService = itemService;
            _itemLocationService = itemLocationService;
            _geolocationService = geolocationService;

            Items = new ObservableCollection<Item>();
            StatusMessage = string.Empty;
            LocationName = "Unknown location";
        }

        public async Task InitializeAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                StatusMessage = "Getting your location...";

                // Get current location
                _currentLocation = await _geolocationService.GetCurrentLocation();
                if (_currentLocation == null)
                {
                    StatusMessage = "Could not determine your location. Please check your device settings.";
                    HasLocation = false;
                    return;
                }

                HasLocation = true;
                LocationName = await _geolocationService.GetLocationName(_currentLocation) ?? "Current Location";

                // Load nearby items
                await LoadNearbyItemsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing nearby items: {ex.Message}");
                StatusMessage = "Error loading nearby items.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadNearbyItemsAsync()
        {
            if (IsBusy || !HasLocation || _currentLocation == null)
                return;

            try
            {
                IsRefreshing = true;
                StatusMessage = $"Finding items within {SearchRadius} km...";
                Items.Clear();

                // Get nearby items
                var nearbyItems = await _itemLocationService.FindItemsNearLocationAsync(_currentLocation, SearchRadius);

                if (nearbyItems.Count == 0)
                {
                    StatusMessage = "No items found nearby. Try increasing your search radius.";
                }
                else
                {
                    StatusMessage = $"Found {nearbyItems.Count} items within {SearchRadius} km.";
                    foreach (var item in nearbyItems)
                    {
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading nearby items: {ex.Message}");
                StatusMessage = "Error loading nearby items.";
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task UpdateSearchRadius()
        {
            if (SearchRadius <= 0)
                SearchRadius = 1;

            await LoadNearbyItemsAsync();
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task ViewItemDetails(Item item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?ItemId={item.Id}");
        }

        [RelayCommand]
        private async Task ViewOnMap(Item item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(ItemMapPage)}?ItemId={item.Id}");
        }
    }
}