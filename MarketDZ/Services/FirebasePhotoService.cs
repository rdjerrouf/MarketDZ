using MarketDZ.Models;
using MarketDZ.Services;
using MarketDZ.Extensions;
using System.Diagnostics;

namespace MarketDZ.Services
{
    public class FirebasePhotoService
    {
        private readonly FirebaseService _firebaseService;
        private readonly IMediaService _mediaService;

        public FirebasePhotoService(FirebaseService firebaseService, IMediaService mediaService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
        }

        // Get all photos for an item
        public async Task<List<ItemPhoto>> GetItemPhotosAsync(int itemId)
        {
            try
            {
                var allPhotos = await GetAllItemPhotosAsync();
                return allPhotos
                    .Where(p => p.ItemId == itemId)
                    .OrderBy(p => p.DisplayOrder)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting item photos: {ex.Message}");
                return new List<ItemPhoto>();
            }
        }

        // Add a new photo
        public async Task<ItemPhoto> AddPhotoAsync(int itemId, FileResult photoFile)
        {
            try
            {
                // Get the item
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    throw new Exception($"Item with ID {itemId} not found");

                // Get existing photos
                var existingPhotos = await GetItemPhotosAsync(itemId);

                // Upload the image to storage/file system
                string photoUrl = await _mediaService.UploadImageAsync(photoFile);

                // Check if this is the first photo
                bool isPrimary = !existingPhotos.Any();

                // Get the highest display order
                int maxDisplayOrder = existingPhotos.Any()
                    ? existingPhotos.Max(p => p.DisplayOrder)
                    : -1;

                // Create new photo record
                var photo = new ItemPhoto
                {
                    Id = await GetNextItemPhotoIdAsync(),
                    ItemId = itemId,
                    Item = item, // Set the required Item property
                    PhotoUrl = photoUrl,
                    IsPrimaryPhoto = isPrimary,
                    UploadedAt = DateTime.UtcNow,
                    DisplayOrder = maxDisplayOrder + 1
                };

                // Add to Firebase
                await _firebaseService.SetAsync($"itemPhotos/{photo.Id}", photo);

                // If this is the first/primary photo, update the item's main photo URL
                if (isPrimary)
                {
                    item.PhotoUrl = photoUrl;
                    item.ImageUrl = photoUrl;
                    await _firebaseService.UpdateItemAsync(item);
                }

                return photo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding photo: {ex.Message}");
                throw;
            }
        }

        // Delete a photo
        public async Task DeletePhotoAsync(int photoId)
        {
            try
            {
                var photo = await GetItemPhotoByIdAsync(photoId);
                if (photo == null)
                    throw new Exception("Photo not found");

                var itemId = photo.ItemId;
                bool wasPrimary = photo.IsPrimaryPhoto;

                // Get the item
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    throw new Exception("Item not found");

                // Delete the file from storage
                await _mediaService.DeleteImageAsync(photo.PhotoUrl);

                // Remove from Firebase
                await _firebaseService.DeleteAsync($"itemPhotos/{photoId}");

                // If this was the primary photo, set a new one
                if (wasPrimary)
                {
                    var allPhotos = await GetItemPhotosAsync(itemId);
                    var newPrimary = allPhotos
                        .Where(p => p.Id != photoId)
                        .OrderBy(p => p.DisplayOrder)
                        .FirstOrDefault();

                    if (newPrimary != null)
                    {
                        // Update new primary photo
                        newPrimary.IsPrimaryPhoto = true;
                        await _firebaseService.SetAsync($"itemPhotos/{newPrimary.Id}", newPrimary);

                        // Update item
                        item.PhotoUrl = newPrimary.PhotoUrl;
                        item.ImageUrl = newPrimary.PhotoUrl;
                    }
                    else
                    {
                        // No photos left
                        item.PhotoUrl = null;
                        item.ImageUrl = null;
                    }

                    await _firebaseService.UpdateItemAsync(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting photo: {ex.Message}");
                throw;
            }
        }

        // Set a photo as primary
        public async Task SetPrimaryPhotoAsync(int photoId)
        {
            try
            {
                var photo = await GetItemPhotoByIdAsync(photoId);
                if (photo == null)
                    throw new Exception("Photo not found");

                var itemId = photo.ItemId;

                // Get the item
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    throw new Exception("Item not found");

                // Get all photos for this item
                var allPhotos = await GetItemPhotosAsync(itemId);

                // Update primary status for all photos
                foreach (var p in allPhotos)
                {
                    bool shouldBePrimary = (p.Id == photoId);
                    if (p.IsPrimaryPhoto != shouldBePrimary)
                    {
                        p.IsPrimaryPhoto = shouldBePrimary;
                        await _firebaseService.SetAsync($"itemPhotos/{p.Id}", p);
                    }
                }

                // Update the item's main photo
                item.PhotoUrl = photo.PhotoUrl;
                item.ImageUrl = photo.PhotoUrl;
                await _firebaseService.UpdateItemAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting primary photo: {ex.Message}");
                throw;
            }
        }

        // Reorder photos
        public async Task ReorderPhotosAsync(int itemId, List<int> photoIds)
        {
            try
            {
                var photos = await GetItemPhotosAsync(itemId);

                // Update display order based on the provided sequence
                for (int i = 0; i < photoIds.Count; i++)
                {
                    var photo = photos.FirstOrDefault(p => p.Id == photoIds[i]);
                    if (photo != null)
                    {
                        photo.DisplayOrder = i;
                        await _firebaseService.SetAsync($"itemPhotos/{photo.Id}", photo);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reordering photos: {ex.Message}");
                throw;
            }
        }

        #region Helper Methods

        private async Task<List<ItemPhoto>> GetAllItemPhotosAsync()
        {
            try
            {
                var photoCollection = await _firebaseService.GetCollectionAsync<ItemPhoto>("itemPhotos");
                return photoCollection?.Select(p => p.Object).ToList() ?? new List<ItemPhoto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting all item photos: {ex.Message}");
                return new List<ItemPhoto>();
            }
        }

        private async Task<ItemPhoto> GetItemPhotoByIdAsync(int photoId)
        {
            try
            {
                return await _firebaseService.GetAsync<ItemPhoto>($"itemPhotos/{photoId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting item photo by ID: {ex.Message}");
                return null;
            }
        }

        private async Task<int> GetNextItemPhotoIdAsync()
        {
            try
            {
                var photos = await GetAllItemPhotosAsync();
                return photos.Any() ? photos.Max(p => p.Id) + 1 : 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion
    }
}