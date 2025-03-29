
namespace MarketDZ.Services
{
    public interface IMediaService
    {
        Task<string> UploadImageAsync(FileResult file);
        Task<string> UploadImageAsync(Stream stream, string fileName);
        Task<bool> DeleteImageAsync(string url);
    }
}