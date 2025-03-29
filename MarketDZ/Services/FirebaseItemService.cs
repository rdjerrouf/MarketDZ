using MarketDZ.Models;
using MarketDZ.Models.Dtos;
using MarketDZ.Models.Filters;
using System.Diagnostics;

namespace MarketDZ.Services
{
    public class FirebaseItemService : IItemService
    {
        private readonly FirebaseService _firebaseService;

        public FirebaseItemService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            try
            {
                // Make sure item has a posted user
                if (item.PostedByUserId > 0 && item.PostedByUser == null)
                {
                    var user = await _firebaseService.GetUserByIdAsync(item.PostedByUserId);
                    if (user == null) return false;
                    item.PostedByUser = user;
                }

                // Add the item to the database
                var result = await _firebaseService.CreateItemAsync(item);
                return result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding item: {ex.Message}");
                return false;
            }
        }

        public async Task<int?> AddItemAsync(int userId, CreateItemDto itemDto)
        {
            try
            {
                // Get the user first
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null) return null;

                // Create the item
                var item = new Item
                {
                    Title = itemDto.Title,
                    Description = itemDto.Description,
                    Price = itemDto.Price,
                    Category = itemDto.Category,
                    PostedByUserId = userId,
                    PostedByUser = user,
                    JobType = itemDto.JobType,
                    ServiceType = itemDto.ServiceType,
                    JobCategory = itemDto.JobCategory,
                    CompanyName = itemDto.CompanyName,
                    JobLocation = itemDto.JobLocation,
                    ApplyMethod = itemDto.ApplyMethod,
                    ApplyContact = itemDto.ApplyContact,
                    ListedDate = DateTime.UtcNow,
                    Status = ItemStatus.Active
                };

                var itemId = await _firebaseService.CreateItemAsync(item);
                if (itemId <= 0) return null;

                if (itemDto.PhotoUrls?.Any() == true)
                {
                    foreach (var photoUrl in itemDto.PhotoUrls.Take(2))
                    {
                        await AddItemPhotoAsync(userId, itemId, photoUrl);
                    }
                }

                return itemId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating item: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateItemAsync(int userId, int itemId, ItemUpdateDto updateDto)
        {
            try
            {
                // Get the current item
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null || item.PostedByUserId != userId)
                    return false;

                // Update properties
                item.Title = updateDto.Title ?? item.Title;
                item.Description = updateDto.Description ?? item.Description;
                item.Price = updateDto.Price;
                item.Category = updateDto.Category ?? item.Category;
                item.JobType = updateDto.JobType;
                item.ServiceType = updateDto.ServiceType;
                item.RentalPeriod = updateDto.RentalPeriod;
                item.AvailableFrom = updateDto.AvailableFrom;
                item.AvailableTo = updateDto.AvailableTo;
                item.JobCategory = updateDto.JobCategory;
                item.CompanyName = updateDto.CompanyName;
                item.JobLocation = updateDto.JobLocation;
                item.ApplyMethod = updateDto.ApplyMethod;
                item.ApplyContact = updateDto.ApplyContact;
                item.ServiceCategory = updateDto.ServiceCategory;
                item.ServiceAvailability = updateDto.ServiceAvailability;
                item.YearsOfExperience = updateDto.YearsOfExperience;
                item.ServiceLocation = updateDto.ServiceLocation;
                item.ForSaleCategory = updateDto.ForSaleCategory;
                item.ForRentCategory = updateDto.ForRentCategory;
                item.State = updateDto.State;
                item.Latitude = updateDto.Latitude;
                item.Longitude = updateDto.Longitude;

                // Save the updated item
                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating item: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                return await _firebaseService.DeleteItemAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting item: {ex.Message}");
                return false;
            }
        }

        public async Task<Item?> GetItemAsync(int id)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(id);
                if (item != null)
                {
                    await IncrementItemViewAsync(id);
                }
                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving item: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            try
            {
                var items = await _firebaseService.GetItemsAsync();
                return items.OrderByDescending(i => i.ListedDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving items: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        public async Task<IEnumerable<Item>> GetUserItemsAsync(int userId)
        {
            try
            {
                var items = await _firebaseService.GetItemsByUserIdAsync(userId);
                return items.OrderByDescending(i => i.ListedDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving user items: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        public Task<IEnumerable<Item>> GetItemsByUserAsync(int userId) => GetUserItemsAsync(userId);

        public async Task<IEnumerable<Item>> SearchItemsAsync(string searchTerm, string? category = null)
        {
            try
            {
                var items = await _firebaseService.GetItemsAsync();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    items = items.Where(i =>
                        i.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        i.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(category))
                {
                    items = items.Where(i => i.Category == category);
                }

                return items.OrderByDescending(i => i.ListedDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching items: {ex.Message}");
                return [];  // Use collection expression
            }
        }

        public async Task<IEnumerable<Item>> SearchByStateAsync(AlState state)
        {
            try
            {
                var items = await _firebaseService.GetItemsAsync();
                return items
                    .Where(i => i.State == state)
                    .OrderByDescending(i => i.ListedDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching by state: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        public async Task<IEnumerable<Item>> SearchByLocationAsync(double latitude, double longitude, double radiusKm)
        {
            try
            {
                var items = await _firebaseService.GetItemsAsync();
                var lat1 = latitude * Math.PI / 180;
                var lon1 = longitude * Math.PI / 180;

                return items
                    .Where(i => i.Latitude != null && i.Longitude != null)
                    .Select(i => new
                    {
                        Item = i,
                        Distance = 6371 * Math.Acos(
                            Math.Sin(lat1) * Math.Sin(i.Latitude!.Value * Math.PI / 180) +
                            Math.Cos(lat1) * Math.Cos(i.Latitude!.Value * Math.PI / 180) *
                            Math.Cos((i.Longitude!.Value * Math.PI / 180) - lon1)
                        )
                    })
                    .Where(x => x.Distance <= radiusKm)
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching by location: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        public async Task<IEnumerable<Item>> SearchByCategoryAndStateAsync(string category, AlState state)
        {
            try
            {
                var items = await _firebaseService.GetItemsAsync();
                return items
                    .Where(i => i.Category == category && i.State == state)
                    .OrderByDescending(i => i.ListedDate);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching by category and state: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        public async Task<IEnumerable<Item>> GetItemsWithFiltersAsync(FilterCriteria criteria)
        {
            try
            {
                var items = await _firebaseService.GetItemsAsync();

                if (criteria.MinPrice.HasValue)
                    items = items.Where(i => i.Price >= criteria.MinPrice.Value);

                if (criteria.MaxPrice.HasValue)
                    items = items.Where(i => i.Price <= criteria.MaxPrice.Value);

                if (criteria.State.HasValue)
                    items = items.Where(i => i.State == criteria.State.Value);

                if (criteria.Categories?.Any() == true)
                    items = items.Where(i => criteria.Categories.Contains(i.Category));

                if (!string.IsNullOrWhiteSpace(criteria.SearchText))
                    items = items.Where(i =>
                        i.Title.Contains(criteria.SearchText) ||
                        i.Description.Contains(criteria.SearchText));

                if (criteria.DateFrom.HasValue)
                    items = items.Where(i => i.ListedDate >= criteria.DateFrom.Value);

                if (criteria.DateTo.HasValue)
                    items = items.Where(i => i.ListedDate <= criteria.DateTo.Value);

                // Apply sorting
                items = criteria.SortBy switch
                {
                    SortOption.PriceLowToHigh => items.OrderBy(i => i.Price),
                    SortOption.PriceHighToLow => items.OrderByDescending(i => i.Price),
                    SortOption.DateNewest => items.OrderByDescending(i => i.ListedDate),
                    SortOption.DateOldest => items.OrderBy(i => i.ListedDate),
                    _ => items.OrderByDescending(i => i.ListedDate)
                };

                return items;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error filtering items: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        // Implementation for favorites and other required methods will be added in future updates
        // These are placeholders to fulfill the interface requirements

        public Task<bool> AddFavoriteAsync(int userId, int itemId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for AddFavoriteAsync not yet implemented");
            return Task.FromResult(false);
        }

        public Task<bool> RemoveFavoriteAsync(int userId, int itemId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for RemoveFavoriteAsync not yet implemented");
            return Task.FromResult(false);
        }

        public Task<bool> AddRatingAsync(int userId, int itemId, int score, string review)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for AddRatingAsync not yet implemented");
            return Task.FromResult(false);
        }

        public Task<IEnumerable<Item>> GetUserFavoriteItemsAsync(int userId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for GetUserFavoriteItemsAsync not yet implemented");
            return Task.FromResult(Enumerable.Empty<Item>());
        }

        public Task<IEnumerable<Rating>> GetUserRatingsAsync(int userId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for GetUserRatingsAsync not yet implemented");
            return Task.FromResult(Enumerable.Empty<Rating>());
        }

        public Task<UserProfileStatistics> GetUserProfileStatisticsAsync(int userId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for GetUserProfileStatisticsAsync not yet implemented");
            return Task.FromResult(new UserProfileStatistics { UserId = userId });
        }

        public async Task<bool> UpdateItemStatusAsync(int userId, int itemId, ItemStatus status)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null || item.PostedByUserId != userId)
                    return false;

                item.Status = status;
                if (status == ItemStatus.Sold || status == ItemStatus.Rented)
                {
                    item.AvailableTo = DateTime.UtcNow;
                }

                return await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating item status: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsItemAvailableAsync(int itemId)
        {
            try
            {
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                return item?.Status == ItemStatus.Active;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking item availability: {ex.Message}");
                return false;
            }
        }

        public Task<bool> AddItemPhotoAsync(int userId, int itemId, string photoUrl)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for AddItemPhotoAsync not yet implemented");
            return Task.FromResult(false);
        }

        public Task<bool> RemoveItemPhotoAsync(int userId, int photoId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for RemoveItemPhotoAsync not yet implemented");
            return Task.FromResult(false);
        }

        public Task<bool> SetPrimaryPhotoAsync(int userId, int photoId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for SetPrimaryPhotoAsync not yet implemented");
            return Task.FromResult(false);
        }

        public Task<IEnumerable<ItemPhoto>> GetItemPhotosAsync(int itemId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for GetItemPhotosAsync not yet implemented");
            return Task.FromResult(Enumerable.Empty<ItemPhoto>());
        }

        public Task<IEnumerable<ItemPerformanceDto>> GetTopPerformingItemsAsync(int count)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for GetTopPerformingItemsAsync not yet implemented");
            return Task.FromResult(Enumerable.Empty<ItemPerformanceDto>());
        }

        public Task<bool> IncrementItemViewAsync(int itemId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for IncrementItemViewAsync not yet implemented");
            return Task.FromResult(true);
        }

        public Task<ItemStatistics?> GetItemStatisticsAsync(int itemId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for GetItemStatisticsAsync not yet implemented");
            return Task.FromResult<ItemStatistics?>(null);
        }

        public Task<bool> RecordItemInquiryAsync(int itemId)
        {
            // Placeholder - will implement in next phase
            Debug.WriteLine("Firebase implementation for RecordItemInquiryAsync not yet implemented");
            return Task.FromResult(true);
        }
    }
}