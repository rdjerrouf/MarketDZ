using MarketDZ.Models;
using MarketDZ.Models.Dtos;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace MarketDZ.Services
{
    /// <summary>
    /// Firebase implementation of the IAuthService interface
    /// </summary>
    public class FirebaseAuthService : IAuthService
    {
        private readonly FirebaseService _firebaseService;
        private readonly IVerificationService _verificationService;
        private static SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);
        private bool _isInitialized;

        public FirebaseAuthService(
            FirebaseService firebaseService,
            IVerificationService verificationService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        }

        /// <summary>
        /// Initialize Firebase connection
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                await _initLock.WaitAsync();
                if (_isInitialized) return;

                Debug.WriteLine("Starting Firebase Auth initialization");

                // Initialize Firebase service
                await _firebaseService.InitializeAsync();

                _isInitialized = true;
                Debug.WriteLine("Firebase Auth initialization completed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firebase Auth initialization error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        /// <summary>
        /// Get a user by their email address
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            try
            {
                // Using your existing _firebaseService correctly
                var users = await _firebaseService.GetCollectionAsync<User>("users");

                // Filter out null values and then search by email
                return users
                    .Where(u => u?.Object != null)
                    .Select(u => u.Object)
                    .FirstOrDefault(u => u.Email?.ToLower() == email.ToLower());
            }
            catch (Firebase.Database.FirebaseException ex)
            {
                // Log the specific Firebase error
                Debug.WriteLine($"Firebase error getting user by email: {ex.Message}");

                // Consider propagating certain exceptions
                if (ex.Message.Contains("Authentication"))
                    throw new UnauthorizedAccessException("Firebase authentication failed", ex);

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user by email: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Update a user's profile information
        /// </summary>
        public async Task<bool> UpdateUserProfileAsync(int userId, string displayName, string profilePicture, string bio)
        {
            var user = await _firebaseService.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.DisplayName = displayName;
            user.ProfilePicture = profilePicture;
            user.Bio = bio;

            return await _firebaseService.UpdateUserAsync(user);
        }

        /// <summary>
        /// Update a user's privacy settings
        /// </summary>
        public async Task<bool> UpdateUserPrivacyAsync(int userId, bool showEmail, bool showPhoneNumber)
        {
            var user = await _firebaseService.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.ShowEmail = showEmail;
            user.ShowPhoneNumber = showPhoneNumber;

            return await _firebaseService.UpdateUserAsync(user);
        }

        /// <summary>
        /// Change a user's password
        /// </summary>
        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            await InitializeAsync();

            try
            {
                // First, verify the current password
                var user = await SignInAsync(email, currentPassword);
                if (user == null)
                {
                    Debug.WriteLine("Current password verification failed");
                    return false;
                }

                // Hash the new password
                user.PasswordHash = PasswordHasher.HashPassword(newPassword);

                // Update the user
                var result = await _firebaseService.UpdateUserAsync(user);

                Debug.WriteLine($"Password change result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Password change error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public async Task<bool> RegisterUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            await InitializeAsync();

            try
            {
                Debug.WriteLine($"\nStarting registration for email: {user.Email}");

                // Check for existing user
                var duplicateUser = await _firebaseService.GetUserByEmailAsync(user.Email);

                if (duplicateUser != null)
                {
                    Debug.WriteLine($"Found duplicate user with email: {user.Email}");
                    return false;
                }

                // Set creation date if not already set
                if (user.CreatedAt == default)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }

                // Hash password
                user.PasswordHash = PasswordHasher.HashPassword(user.PasswordHash);

                // Save user
                var result = await _firebaseService.CreateUserAsync(user);

                Debug.WriteLine($"Registration completed. Result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Registration error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Authenticate a user with email and password
        /// </summary>
        public async Task<User?> SignInAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Email and password are required");
            }

            await InitializeAsync();

            try
            {
                Debug.WriteLine($"\nAttempting sign in for email: {email}");

                // Find user by email
                var user = await _firebaseService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    Debug.WriteLine($"No user found with email: {email}");
                    return null;
                }

                // Verify password
                var isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);
                Debug.WriteLine($"Password verification result: {isPasswordValid}");

                if (isPasswordValid)
                {
                    // Store the user ID in secure storage for session persistence
                    await SecureStorage.SetAsync("userId", user.Id.ToString());
                    Debug.WriteLine($"Stored user ID {user.Id} in SecureStorage");

                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sign in error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Get the current logged-in user
        /// </summary>
        public async Task<User?> GetCurrentUserAsync()
        {
            try
            {
                // Attempt to retrieve the stored user ID from secure storage
                string? storedUserId = await SecureStorage.GetAsync("userId");

                // Check if we have a stored user ID
                if (string.IsNullOrEmpty(storedUserId))
                {
                    Debug.WriteLine("No stored user ID found. User is not logged in.");
                    return null;
                }

                // Attempt to parse the stored user ID to an integer
                if (!int.TryParse(storedUserId, out int userId))
                {
                    Debug.WriteLine("Stored user ID is invalid. Clearing stored ID.");
                    await SecureStorage.SetAsync("userId", string.Empty);
                    return null;
                }

                // Attempt to retrieve the user from Firebase
                User? user = await _firebaseService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    Debug.WriteLine($"User with ID {userId} not found in database. Clearing stored ID.");
                    await SecureStorage.SetAsync("userId", string.Empty);
                    return null;
                }

                Debug.WriteLine($"Successfully retrieved current user. ID: {user.Id}, Email: {user.Email}");
                return user;
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                Debug.WriteLine($"Error in GetCurrentUserAsync: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get the ID of the current logged-in user
        /// </summary>
        public async Task<int> GetCurrentUserIdAsync()
        {
            try
            {
                // Attempt to retrieve the stored user ID from secure storage
                string? storedUserId = await SecureStorage.GetAsync("userId");

                // Check if we have a stored user ID
                if (string.IsNullOrEmpty(storedUserId))
                {
                    Debug.WriteLine("No stored user ID found. User is not logged in.");
                    throw new InvalidOperationException("No user is currently logged in.");
                }

                // Attempt to parse the stored user ID to an integer
                if (!int.TryParse(storedUserId, out int userId))
                {
                    Debug.WriteLine("Stored user ID is invalid.");
                    throw new InvalidOperationException("Invalid stored user ID.");
                }

                // Verify the user exists
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Debug.WriteLine($"User with ID {userId} not found in database.");
                    throw new InvalidOperationException("Current user not found in database.");
                }

                Debug.WriteLine($"Successfully retrieved current user ID: {userId}");
                return userId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetCurrentUserIdAsync: {ex.Message}");
                throw; // Re-throw to allow caller to handle the error
            }
        }

        /// <summary>
        /// Update a user's contact information
        /// </summary>
        public async Task<bool> UpdateUserContactInfoAsync(int userId, string? phoneNumber, string? city, string? province)
        {
            try
            {
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null) return false;

                // Update contact information
                user.PhoneNumber = phoneNumber;
                user.City = city;
                user.Province = province;

                return await _firebaseService.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user contact info: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reset a user's password
        /// </summary>
        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            await InitializeAsync();

            try
            {
                var user = await _firebaseService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    Debug.WriteLine($"No user found with email: {email}");
                    return false;
                }

                // Hash the new password
                user.PasswordHash = PasswordHasher.HashPassword(newPassword);

                // Update the user
                var result = await _firebaseService.UpdateUserAsync(user);

                Debug.WriteLine($"Password reset result: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Password reset error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Check if an email is already registered
        /// </summary>
        public async Task<bool> IsEmailRegisteredAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            await InitializeAsync();

            try
            {
                var user = await _firebaseService.GetUserByEmailAsync(email);
                return user != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking email registration: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get a user's profile information
        /// </summary>
        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null) return null;

                // Get user's posted items count
                var postedItems = await _firebaseService.GetItemsByUserIdAsync(userId);

                // In Firebase, we don't have direct access to FavoriteItems via Include
                // So we'll need to implement this separately in the FirebaseService
                int favoriteItemsCount = 0; // This would need to be implemented

                return new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    ProfilePicture = user.ProfilePicture,
                    Bio = user.Bio,
                    CreatedAt = user.CreatedAt,
                    PhoneNumber = user.ShowPhoneNumber ? user.PhoneNumber : null,
                    City = user.City,
                    Province = user.Province,
                    PostedItemsCount = postedItems.Count(),
                    FavoriteItemsCount = favoriteItemsCount
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving user profile: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Send an email verification token to a user
        /// </summary>
        public async Task<bool> SendEmailVerificationTokenAsync(int userId)
        {
            try
            {
                // Find the user
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    Debug.WriteLine($"User not found. UserId: {userId}");
                    return false;
                }

                // Generate verification token
                string token = await _verificationService.GenerateVerificationTokenAsync(
                    userId,
                    VerificationType.EmailVerification
                );

                // TODO: Implement actual email sending logic
                Debug.WriteLine($"Verification token generated for user {userId}: {token}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending email verification: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verify a user's email with a token
        /// </summary>
        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                var verificationToken = await _firebaseService.GetVerificationTokenAsync(token);

                if (verificationToken == null || verificationToken.IsUsed)
                    return false;

                var user = await _firebaseService.GetUserByIdAsync(verificationToken.UserId);
                if (user == null)
                    return false;

                user.IsEmailVerified = true;
                user.EmailVerifiedAt = DateTime.UtcNow;

                // Update the user
                await _firebaseService.UpdateUserAsync(user);

                // Mark token as used
                await _verificationService.MarkTokenAsUsedAsync(token);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error verifying email: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Check if a user's email is verified
        /// </summary>
        public async Task<bool> IsEmailVerifiedAsync(int userId)
        {
            try
            {
                var user = await _firebaseService.GetUserByIdAsync(userId);
                return user?.IsEmailVerified ?? false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking email verification: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Confirm a user's email with a token
        /// </summary>
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                if (!int.TryParse(userId, out int userIdInt))
                    return false;

                var verificationToken = await _firebaseService.GetVerificationTokenAsync(token);

                if (verificationToken == null || verificationToken.UserId != userIdInt || verificationToken.IsUsed)
                    return false;

                var user = await _firebaseService.GetUserByIdAsync(userIdInt);
                if (user == null)
                    return false;

                user.IsEmailVerified = true;
                user.EmailVerifiedAt = DateTime.UtcNow;

                // Update the user
                await _firebaseService.UpdateUserAsync(user);

                // Mark token as used
                await _verificationService.MarkTokenAsUsedAsync(token);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error confirming email: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Generate an email verification token for a user
        /// </summary>
        public async Task<string> GenerateEmailVerificationTokenAsync(User user)
        {
            try
            {
                var token = await _verificationService.GenerateVerificationTokenAsync(
                    user.Id,
                    VerificationType.EmailVerification);

                return token;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating verification token: {ex.Message}");
                throw;
            }
        }
    }
}