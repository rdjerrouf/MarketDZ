using MarketDZ.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace MarketDZ.Services
{
    public class FirebaseVerificationService : IVerificationService
    {
        private readonly FirebaseService _firebaseService;

        public FirebaseVerificationService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        }

        public async Task<string> GenerateVerificationTokenAsync(int userId, VerificationType type)
        {
            try
            {
                // Generate a random token
                string token = GenerateRandomToken();

                // Get the user first
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception($"User with ID {userId} not found");
                }

                // Create and store the verification token
                var verificationToken = new VerificationToken
                {
                    UserId = userId,
                    User = user,  // Set the required User property
                    Token = token,
                    Type = type,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24), // Token valid for 24 hours
                    IsUsed = false
                };

                bool success = await _firebaseService.CreateVerificationTokenAsync(verificationToken);

                if (!success)
                {
                    throw new Exception("Failed to store verification token");
                }

                return token;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating verification token: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ValidateVerificationTokenAsync(string token, VerificationType type)
        {
            try
            {
                var verificationToken = await _firebaseService.GetVerificationTokenAsync(token);

                if (verificationToken == null)
                {
                    Debug.WriteLine("Token not found");
                    return false;
                }

                if (verificationToken.Type != type)
                {
                    Debug.WriteLine($"Token type mismatch. Expected: {type}, Actual: {verificationToken.Type}");
                    return false;
                }

                if (verificationToken.IsUsed)
                {
                    Debug.WriteLine("Token has already been used");
                    return false;
                }

                if (verificationToken.ExpiresAt < DateTime.UtcNow)
                {
                    Debug.WriteLine("Token has expired");
                    return false;
                }

                Debug.WriteLine("Token validated successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error validating verification token: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MarkTokenAsUsedAsync(string token)
        {
            try
            {
                var verificationToken = await _firebaseService.GetVerificationTokenAsync(token);

                if (verificationToken == null)
                {
                    Debug.WriteLine("Token not found");
                    return false;
                }

                verificationToken.IsUsed = true;
                bool success = await _firebaseService.CreateVerificationTokenAsync(verificationToken);

                Debug.WriteLine($"Token marked as used: {success}");
                return success;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error marking token as used: {ex.Message}");
                return false;
            }
        }

        private string GenerateRandomToken()
        {
            // Generate a random 32-byte token
            using (var rng = RandomNumberGenerator.Create())
            {
                var tokenBytes = new byte[32];
                rng.GetBytes(tokenBytes);
                return Convert.ToBase64String(tokenBytes)
                    .Replace("/", "_")
                    .Replace("+", "-")
                    .Replace("=", "");
            }
        }
    }
}