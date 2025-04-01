using System.Diagnostics;

namespace MarketDZ.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Opens the device's image picker and allows the user to select an image
        /// </summary>
        /// <returns>A FileResult if an image was picked, null otherwise</returns>
        public static async Task<FileResult> PickImageAsync()
        {
            try
            {
                var options = new PickOptions
                {
                    PickerTitle = "Select an Image",
                    FileTypes = FilePickerFileType.Images
                };

                var result = await FilePicker.Default.PickAsync(options);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Image picking failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Takes a photo using the device camera
        /// </summary>
        /// <returns>A FileResult if a photo was taken, null otherwise</returns>
        public static async Task<FileResult> TakePhotoAsync()
        {
            try
            {
                // Request camera permission
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                        return null;
                }

                var photo = await MediaPicker.Default.CapturePhotoAsync();
                return photo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Camera capture failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a unique filename for an image based on its purpose
        /// </summary>
        public static string CreateUniqueFileName(string purpose, string extension = ".jpg")
        {
            if (!extension.StartsWith("."))
                extension = "." + extension;

            return $"{purpose.ToLower()}_photo_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
        }

        /// <summary>
        /// Compresses an image for upload
        /// </summary>
        public static async Task<Stream> CompressImageAsync(Stream imageStream, int quality = 80)
        {
            // This is a placeholder method - in a full implementation, you would use a library
            // like SkiaSharp or ImageSharp to actually compress the image

            // For now, we'll just return the original stream
            var memoryStream = new MemoryStream();
            imageStream.Position = 0;
            await imageStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}