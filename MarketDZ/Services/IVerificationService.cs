using MarketDZ.Models;

namespace MarketDZ.Services
{
    public interface IVerificationService
    {
        Task<string> GenerateVerificationTokenAsync(int userId, VerificationType type);
        Task<bool> ValidateVerificationTokenAsync(string token, VerificationType type);
        Task<bool> MarkTokenAsUsedAsync(string token);
    }
}