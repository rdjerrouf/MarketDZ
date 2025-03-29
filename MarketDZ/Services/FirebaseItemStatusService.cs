using MarketDZ.Models;
using System.Diagnostics;

namespace MarketDZ.Services
{
    public class FirebaseItemStatusService
    {
        private readonly FirebaseService _firebaseService;

        public FirebaseItemStatusService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        }

        // Mark an item as active
        public async Task<bool> MarkAsActiveAsync(int itemId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    return false;

                item.Status = ItemStatus.Active;
                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error marking item as active: {ex.Message}");
                return false;
            }
        }

        // Mark an item as sold
        public async Task<bool> MarkAsSoldAsync(int itemId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    return false;

                item.Status = ItemStatus.Sold;
                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error marking item as sold: {ex.Message}");
                return false;
            }
        }

        // Mark an item as rented
        public async Task<bool> MarkAsRentedAsync(int itemId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    return false;

                item.Status = ItemStatus.Rented;
                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error marking item as rented: {ex.Message}");
                return false;
            }
        }

        // Mark an item as unavailable
        public async Task<bool> MarkAsUnavailableAsync(int itemId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    return false;

                item.Status = ItemStatus.Unavailable;
                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error marking item as unavailable: {ex.Message}");
                return false;
            }
        }

        // Check if current user is the owner of an item
        public async Task<bool> IsItemOwnerAsync(int itemId, int userId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                return item != null && item.PostedByUserId == userId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking item ownership: {ex.Message}");
                return false;
            }
        }
    }
}