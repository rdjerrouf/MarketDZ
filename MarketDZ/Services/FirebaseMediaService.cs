using MarketDZ.Models;
using MarketDZ.Models.Dtos;
using System.Diagnostics;


namespace MarketDZ.Services
{
    public class FirebaseMediaService : IMediaService
    {
        private readonly FirebaseService _firebaseService;
        private readonly string _storageBucket;

        public FirebaseMediaService(FirebaseService firebaseService, string storageBucket = "marketdz-a6db7.appspot.com")
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _storageBucket = storageBucket;
        }

        public async Task<string> UploadImageAsync(FileResult file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            try
            {
                // Open the file stream
                using var stream = await file.OpenReadAsync();
                return await UploadImageAsync(stream, file.FileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error uploading image from file: {ex.Message}");
                throw;
            }
        }

        public async Task<string> UploadImageAsync(Stream stream, string fileName)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name must not be empty", nameof(fileName));

            try
            {
                // Convert stream to bytes
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();

                // Generate a unique file name to avoid collisions
                var uniqueFileName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";
                var storagePath = $"images/{uniqueFileName}";

                // Upload to Firebase Storage
                // NOTE: This is a placeholder. You'll need to implement Firebase Storage functionality
                // or use a third-party library like FirebaseStorage.net

                // For now, let's simulate storage by saving it to a Firebase Realtime Database node
                // In a real implementation, you would use Firebase Storage instead
                var imageInfo = new
                {
                    FileName = uniqueFileName,
                    ContentType = GetContentType(fileName),
                    UploadedAt = DateTime.UtcNow,
                    // In a real implementation, you'd store the actual image in Firebase Storage
                    // and only store metadata in the Realtime Database
                    // We're not storing the actual bytes here since that's not efficient in Realtime Database
                };

                // Save metadata to Firebase
                await _firebaseService.AddAsync("imageMetadata", imageInfo);

                // In a real implementation with Firebase Storage, you'd return the download URL
                // For now, return a placeholder URL
                return $"https://{_storageBucket}/images/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error uploading image: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            try
            {
                // Extract the file name from the URL
                var uri = new Uri(url);
                var pathSegments = uri.AbsolutePath.Split('/');
                var fileName = pathSegments.LastOrDefault();

                if (string.IsNullOrEmpty(fileName))
                    return false;

                // In a real implementation, you would delete from Firebase Storage
                // For now, we'll just simulate by removing metadata from Realtime Database

                // Find the image metadata by file name
                var imageMetadata = await _firebaseService.GetCollectionAsync<dynamic>("imageMetadata");
                var imageToDelete = imageMetadata.FirstOrDefault(i =>
                    i.Object.FileName.ToString() == fileName);

                if (imageToDelete != null)
                {
                    // Delete the metadata
                    await _firebaseService.DeleteAsync($"imageMetadata/{imageToDelete.Key}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting image: {ex.Message}");
                return false;
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
