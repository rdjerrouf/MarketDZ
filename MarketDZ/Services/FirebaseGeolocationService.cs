using MarketDZ.Models;
using System.Diagnostics;
using Microsoft.Maui.Devices.Sensors;

namespace MarketDZ.Services
{
    public class FirebaseGeolocationService : IGeolocationService
    {
        private readonly IGeolocation _geolocation;
        private readonly FirebaseService _firebaseService;

        public FirebaseGeolocationService(IGeolocation geolocation, FirebaseService firebaseService)
        {
            _geolocation = geolocation ?? throw new ArgumentNullException(nameof(geolocation));
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        }

        public async Task<Location?> GetCurrentLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var mauilocation = await _geolocation.GetLocationAsync(request);

                if (mauilocation != null)
                {
                    return new Location
                    {
                        Latitude = mauilocation.Latitude,
                        Longitude = mauilocation.Longitude,
                        Accuracy = mauilocation.Accuracy ?? 0
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting current location: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetLocationName(Location location)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    return $"{placemark.Locality}, {placemark.AdminArea}";
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting location name: {ex.Message}");
                return null;
            }
        }

        public async Task<Location?> GetLocationFromAddress(string address)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync(address);
                var location = locations?.FirstOrDefault();

                if (location != null)
                {
                    return new Location
                    {
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                        // Note: We removed the Address property since it doesn't exist in your model
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting location from address: {ex.Message}");
                return null;
            }
        }

        public double CalculateDistance(Location location1, Location location2)
        {
            // Haversine formula for calculating distance between two points on Earth
            const double radius = 6371; // Earth's radius in km

            var lat1 = location1.Latitude * Math.PI / 180;
            var lon1 = location1.Longitude * Math.PI / 180;
            var lat2 = location2.Latitude * Math.PI / 180;
            var lon2 = location2.Longitude * Math.PI / 180;

            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = radius * c;

            return distance;
        }

        public List<Item> FindItemsWithinRadius(List<Item> items, Location currentLocation, double radiusKm)
        {
            try
            {
                return items.Where(item =>
                {
                    if (!item.Latitude.HasValue || !item.Longitude.HasValue)
                        return false;

                    var itemLocation = new Location
                    {
                        Latitude = item.Latitude.Value,
                        Longitude = item.Longitude.Value
                    };

                    var distance = CalculateDistance(currentLocation, itemLocation);
                    return distance <= radiusKm;
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error finding items within radius: {ex.Message}");
                return new List<Item>();
            }
        }

        public List<Item> SortItemsByDistance(List<Item> items, Location currentLocation)
        {
            try
            {
                return items.Where(item => item.Latitude.HasValue && item.Longitude.HasValue)
                    .Select(item =>
                    {
                        var itemLocation = new Location
                        {
                            Latitude = item.Latitude!.Value, // Add ! to suppress warning
                            Longitude = item.Longitude!.Value // Add ! to suppress warning
                        };
                        var distance = CalculateDistance(currentLocation, itemLocation);
                        return new { Item = item, Distance = distance };
                    })
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Item)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sorting items by distance: {ex.Message}");
                return items.ToList(); // Return original list on error
            }
        }
    }
}