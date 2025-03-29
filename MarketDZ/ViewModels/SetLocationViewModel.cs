using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MarketDZ.Services;
using Microsoft.Maui.Devices.Sensors;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MarketDZ.ViewModels
{
    public partial class SetLocationViewModel : ObservableObject
    {
        private readonly IItemLocationService _itemLocationService;
        private readonly IGeolocationService _geolocationService;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get => _selectedLocation;
            set => SetProperty(ref _selectedLocation, value);
        }

        private string _locationName;
        public string LocationName
        {
            get => _locationName;
            set => SetProperty(ref _locationName, value);
        }

        private bool _hasSelectedLocation;
        public bool HasSelectedLocation
        {
            get => _hasSelectedLocation;
            set => SetProperty(ref _hasSelectedLocation, value);
        }

        private string _searchAddress = string.Empty;
        public string SearchAddress
        {
            get => _searchAddress;
            set => SetProperty(ref _searchAddress, value);
        }

        private int _itemId;

        public SetLocationViewModel(IItemLocationService itemLocationService, IGeolocationService geolocationService)
        {
            _itemLocationService = itemLocationService;
            _geolocationService = geolocationService;
            _selectedLocation = new Location(); // Initialize _selectedLocation
            _locationName = "No location selected"; // Initialize _locationName
            SearchAddress = string.Empty;
        }

        public async Task InitializeAsync(int itemId)
        {
            try
            {
                IsBusy = true;
                _itemId = itemId;

                // Check if the item already has a location
                var itemLocation = await _itemLocationService.GetItemLocationAsync(itemId);
                if (itemLocation != null)
                {
                    SelectedLocation = new Location(itemLocation.Latitude, itemLocation.Longitude);
                    LocationName = itemLocation.LocationName ?? "Location set";
                    HasSelectedLocation = true;
                }
                else
                {
                    HasSelectedLocation = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing location view: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<Location?> GetCurrentLocationAsync()
        {
            return await _geolocationService.GetCurrentLocation();
        }

        public async void SetSelectedLocation(Location location)
        {
            try
            {
                SelectedLocation = location;
                HasSelectedLocation = true;

                // Update location name by reverse geocoding
                LocationName = "Getting address...";
                var name = await _geolocationService.GetLocationName(location);
                LocationName = name ?? "Unknown location";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting location: {ex.Message}");
                LocationName = "Error getting address";
            }
        }

        [RelayCommand]
        private async Task UseCurrentLocation()
        {
            try
            {
                IsBusy = true;

                var location = await _geolocationService.GetCurrentLocation();
                if (location != null)
                {
                    SetSelectedLocation(location);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Could not determine your current location. Please check your device location settings and permissions.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting current location: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to access location services. Please ensure location permissions are granted.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SearchAddressCommand()
        {
            if (string.IsNullOrWhiteSpace(SearchAddress))
                return;

            try
            {
                IsBusy = true;

                var location = await _geolocationService.GetLocationFromAddress(SearchAddress);
                if (location != null)
                {
                    SetSelectedLocation(location);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "Could not find the specified address. Please try a different search term.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching address: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to search for the address.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveLocation()
        {
            if (!HasSelectedLocation)
                return;

            try
            {
                IsBusy = true;

                var success = await _itemLocationService.SaveItemLocationAsync(_itemId, SelectedLocation);
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Location saved successfully!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to save location. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving location: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "An error occurred while saving the location.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}