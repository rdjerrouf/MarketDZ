using MarketDZ.Models;
using System.Diagnostics;
using System.Text;

namespace MarketDZ.Services
{
    public class FirebaseItemLocationService : IItemLocationService
    {
        private readonly FirebaseService _firebaseService;
        private readonly IGeolocationService _geolocationService;

        public FirebaseItemLocationService(
            FirebaseService firebaseService,
            IGeolocationService geolocationService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _geolocationService = geolocationService ?? throw new ArgumentNullException(nameof(geolocationService));
        }

        public async Task<bool> SaveItemLocationAsync(int itemId, Location location)
        {
            try
            {
                // Get the item first
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null) return false;

                // Update item with location data
                item.Latitude = location.Latitude;
                item.Longitude = location.Longitude;

                // Try to get address from coordinates and store in item description or other field if needed
                var locationName = await _geolocationService.GetLocationName(location);
                if (!string.IsNullOrEmpty(locationName))
                {
                    // Store location name in appropriate field
                    // For example, update JobLocation or ServiceLocation based on item type
                    if (!string.IsNullOrEmpty(item.Category))
                    {
                        switch (item.Category.ToLower())
                        {
                            case "jobs":
                                item.JobLocation = locationName;
                                break;
                            case "services":
                                item.ServiceLocation = locationName;
                                break;
                            default:
                                // Store in another field or in state
                                item.State = GetStateFromLocationName(locationName);
                                break;
                        }
                    }
                }

                // Save the updated item
                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving item location: {ex.Message}");
                return false;
            }
        }

        public async Task<ItemLocation?> GetItemLocationAsync(int itemId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null || !item.Latitude.HasValue || !item.Longitude.HasValue)
                    return null;

                // Determine the best address string to use based on item category
                string locationText = DetermineLocationText(item);

                return new ItemLocation
                {
                    ItemId = item.Id,
                    Latitude = item.Latitude.Value,
                    Longitude = item.Longitude.Value
                    // We removed Address since it doesn't exist in your model
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting item location: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteItemLocationAsync(int itemId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null) return false;

                // Clear location data
                item.Latitude = null;
                item.Longitude = null;

                // Clear location-related fields
                item.JobLocation = null;
                item.ServiceLocation = null;

                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting item location: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Item>> FindItemsNearLocationAsync(Location location, double radiusKm)
        {
            try
            {
                // Get all items
                var allItems = (await _firebaseService.GetItemsAsync()).ToList();

                // Filter items within radius
                return _geolocationService.FindItemsWithinRadius(allItems, location, radiusKm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error finding items near location: {ex.Message}");
                return new List<Item>();
            }
        }

        public async Task<List<Item>> FindNearbyItemsAsync(double radiusKm)
        {
            try
            {
                // Get current location
                var currentLocation = await _geolocationService.GetCurrentLocation();
                if (currentLocation == null)
                    return new List<Item>();

                // Find items near current location
                return await FindItemsNearLocationAsync(currentLocation, radiusKm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error finding nearby items: {ex.Message}");
                return new List<Item>();
            }
        }

        public async Task<List<Item>> SortItemsByDistanceAsync(List<Item> items)
        {
            try
            {
                // Get current location
                var currentLocation = await _geolocationService.GetCurrentLocation();
                if (currentLocation == null)
                    return items.ToList(); // Return unsorted list if location not available

                // Sort by distance
                return _geolocationService.SortItemsByDistance(items, currentLocation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sorting items by distance: {ex.Message}");
                return items.ToList(); // Return original list on error
            }
        }

        // Helper method to determine the best location text to show
        private string DetermineLocationText(Item item)
        {
            if (!string.IsNullOrEmpty(item.JobLocation))
                return item.JobLocation;

            if (!string.IsNullOrEmpty(item.ServiceLocation))
                return item.ServiceLocation;

            // Fall back to state if available
            if (item.State.HasValue)
                return item.State.Value.ToString();

            return "Unknown location"; // Ensure we always return a string
        }
        // Helper method to determine state from location name

        private AlState GetStateFromLocationName(string locationName)
        {
            // Map from location names to Algerian provinces
            if (locationName.Contains("Adrar", StringComparison.OrdinalIgnoreCase))
                return AlState.Adrar;
            if (locationName.Contains("Ain Defla", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Ain_Defla", StringComparison.OrdinalIgnoreCase))
                return AlState.Ain_Defla;
            if (locationName.Contains("Ain Temouchent", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Ain_Temouchent", StringComparison.OrdinalIgnoreCase))
                return AlState.Ain_Temouchent;
            if (locationName.Contains("Alger", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Algiers", StringComparison.OrdinalIgnoreCase))
                return AlState.Alger;
            if (locationName.Contains("Annaba", StringComparison.OrdinalIgnoreCase))
                return AlState.Annaba;
            if (locationName.Contains("Batna", StringComparison.OrdinalIgnoreCase))
                return AlState.Batna;
            if (locationName.Contains("Bechar", StringComparison.OrdinalIgnoreCase))
                return AlState.Bechar;
            if (locationName.Contains("Bejaia", StringComparison.OrdinalIgnoreCase))
                return AlState.Bejaia;
            if (locationName.Contains("Beni Abes", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Beni_Abes", StringComparison.OrdinalIgnoreCase))
                return AlState.Beni_Abes;
            if (locationName.Contains("Biskra", StringComparison.OrdinalIgnoreCase))
                return AlState.Biskra;
            if (locationName.Contains("Blida", StringComparison.OrdinalIgnoreCase))
                return AlState.Blida;
            if (locationName.Contains("Bordj Badji Mokhtar", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Bordj_Badji_Mokhtar", StringComparison.OrdinalIgnoreCase))
                return AlState.Bordj_Badji_Mokhtar;
            if (locationName.Contains("Bordj Bou Arreridj", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Bordj_Bou_Arreridj", StringComparison.OrdinalIgnoreCase))
                return AlState.Bordj_Bou_Arreridj;
            if (locationName.Contains("Bouira", StringComparison.OrdinalIgnoreCase))
                return AlState.Bouira;
            if (locationName.Contains("Boumerdes", StringComparison.OrdinalIgnoreCase))
                return AlState.Boumerdes;
            if (locationName.Contains("Chlef", StringComparison.OrdinalIgnoreCase))
                return AlState.Chlef;
            if (locationName.Contains("Constantine", StringComparison.OrdinalIgnoreCase))
                return AlState.Constantine;
            if (locationName.Contains("Djanet", StringComparison.OrdinalIgnoreCase))
                return AlState.Djanet;
            if (locationName.Contains("Djelfa", StringComparison.OrdinalIgnoreCase))
                return AlState.Djelfa;
            if (locationName.Contains("El Bayadh", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("El_Bayadh", StringComparison.OrdinalIgnoreCase))
                return AlState.El_Bayadh;
            if (locationName.Contains("El MGhair", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("El_MGhair", StringComparison.OrdinalIgnoreCase))
                return AlState.El_MGhair;
            if (locationName.Contains("El Meniaa", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("El_Meniaa", StringComparison.OrdinalIgnoreCase))
                return AlState.El_Meniaa;
            if (locationName.Contains("El Oued", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("El_Oued", StringComparison.OrdinalIgnoreCase))
                return AlState.El_Oued;
            if (locationName.Contains("El Tarf", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("El_Tarf", StringComparison.OrdinalIgnoreCase))
                return AlState.El_Tarf;
            if (locationName.Contains("Ghardaia", StringComparison.OrdinalIgnoreCase))
                return AlState.Ghardaia;
            if (locationName.Contains("Guelma", StringComparison.OrdinalIgnoreCase))
                return AlState.Guelma;
            if (locationName.Contains("Illizi", StringComparison.OrdinalIgnoreCase))
                return AlState.Illizi;
            if (locationName.Contains("In Guezzam", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("In_Guezzam", StringComparison.OrdinalIgnoreCase))
                return AlState.In_Guezzam;
            if (locationName.Contains("In Salah", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("In_Salah", StringComparison.OrdinalIgnoreCase))
                return AlState.In_Salah;
            if (locationName.Contains("Jijel", StringComparison.OrdinalIgnoreCase))
                return AlState.Jijel;
            if (locationName.Contains("Khenchela", StringComparison.OrdinalIgnoreCase))
                return AlState.Khenchela;
            if (locationName.Contains("Laghouat", StringComparison.OrdinalIgnoreCase))
                return AlState.Laghouat;
            if (locationName.Contains("MSila", StringComparison.OrdinalIgnoreCase))
                return AlState.MSila;
            if (locationName.Contains("Mascara", StringComparison.OrdinalIgnoreCase))
                return AlState.Mascara;
            if (locationName.Contains("Medea", StringComparison.OrdinalIgnoreCase))
                return AlState.Medea;
            if (locationName.Contains("Mila", StringComparison.OrdinalIgnoreCase))
                return AlState.Mila;
            if (locationName.Contains("Mostaganem", StringComparison.OrdinalIgnoreCase))
                return AlState.Mostaganem;
            if (locationName.Contains("Naama", StringComparison.OrdinalIgnoreCase))
                return AlState.Naama;
            if (locationName.Contains("Oran", StringComparison.OrdinalIgnoreCase))
                return AlState.Oran;
            if (locationName.Contains("Ouargla", StringComparison.OrdinalIgnoreCase))
                return AlState.Ouargla;
            if (locationName.Contains("Ouled Djellal", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Ouled_Djellal", StringComparison.OrdinalIgnoreCase))
                return AlState.Ouled_Djellal;
            if (locationName.Contains("Oum El Bouaghi", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Oum_El_Bouaghi", StringComparison.OrdinalIgnoreCase))
                return AlState.Oum_El_Bouaghi;
            if (locationName.Contains("Relizane", StringComparison.OrdinalIgnoreCase))
                return AlState.Relizane;
            if (locationName.Contains("Saida", StringComparison.OrdinalIgnoreCase))
                return AlState.Saida;
            if (locationName.Contains("Setif", StringComparison.OrdinalIgnoreCase))
                return AlState.Setif;
            if (locationName.Contains("Sidi Bel Abbes", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Sidi_Bel_Abbes", StringComparison.OrdinalIgnoreCase))
                return AlState.Sidi_Bel_Abbes;
            if (locationName.Contains("Skikda", StringComparison.OrdinalIgnoreCase))
                return AlState.Skikda;
            if (locationName.Contains("Souk Ahras", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Souk_Ahras", StringComparison.OrdinalIgnoreCase))
                return AlState.Souk_Ahras;
            if (locationName.Contains("Tamanrasset", StringComparison.OrdinalIgnoreCase))
                return AlState.Tamanrasset;
            if (locationName.Contains("Tebessa", StringComparison.OrdinalIgnoreCase))
                return AlState.Tebessa;
            if (locationName.Contains("Tiaret", StringComparison.OrdinalIgnoreCase))
                return AlState.Tiaret;
            if (locationName.Contains("Timimoun", StringComparison.OrdinalIgnoreCase))
                return AlState.Timimoun;
            if (locationName.Contains("Tindouf", StringComparison.OrdinalIgnoreCase))
                return AlState.Tindouf;
            if (locationName.Contains("Tipaza", StringComparison.OrdinalIgnoreCase))
                return AlState.Tipaza;
            if (locationName.Contains("Tissemsilt", StringComparison.OrdinalIgnoreCase))
                return AlState.Tissemsilt;
            if (locationName.Contains("Tizi Ouzou", StringComparison.OrdinalIgnoreCase) ||
                locationName.Contains("Tizi_Ouzou", StringComparison.OrdinalIgnoreCase))
                return AlState.Tizi_Ouzou;
            if (locationName.Contains("Tlemcen", StringComparison.OrdinalIgnoreCase))
                return AlState.Tlemcen;
            if (locationName.Contains("Touggourt", StringComparison.OrdinalIgnoreCase))
                return AlState.Touggourt;

            // Default to Alger (capital) if no match found
            return AlState.Alger;
        }
    }
}
