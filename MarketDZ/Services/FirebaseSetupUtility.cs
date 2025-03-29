using MarketDZ.Models;
using System.Diagnostics;
using System.Text;

namespace MarketDZ.Services
{
    /// <summary>
    /// Utility for initializing and setting up Firebase data
    /// </summary>
    public class FirebaseSetupUtility
    {
        private readonly FirebaseService _firebaseService;
        private readonly IMediaService _mediaService;
        private StringBuilder _setupLog = new StringBuilder();

        /// <summary>
        /// Constructor for the setup utility
        /// </summary>
        /// <param name="firebaseService">Firebase service</param>
        /// <param name="mediaService">Media service for handling file transfers</param>
        public FirebaseSetupUtility(
            FirebaseService firebaseService,
            IMediaService mediaService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
        }

        /// <summary>
        /// Get the log of the setup process
        /// </summary>
        public string SetupLog => _setupLog.ToString();

        /// <summary>
        /// Initialize Firebase with basic data structures
        /// </summary>
        public async Task<bool> InitializeFirebaseAsync()
        {
            try
            {
                // Initialize Firebase
                await _firebaseService.InitializeAsync();
                LogInfo("Firebase initialized");

                // Create a test connection entry
                await _firebaseService.SetAsync("connectionTest", "Firebase connection successful: " + DateTime.UtcNow.ToString());
                LogInfo("Connection test entry created");

                // Create a sample admin user for testing
                await CreateSampleAdminUserAsync();

                LogInfo("Firebase setup completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Firebase setup failed: {ex.Message}");
                LogError(ex.StackTrace ?? "No stack trace available");
                return false;
            }
        }

        /// <summary>
        /// Create a sample admin user for testing
        /// </summary>
        private async Task<bool> CreateSampleAdminUserAsync()
        {
            try
            {
                LogInfo("Creating sample admin user...");

                // Check if the admin user already exists
                var existingUser = await _firebaseService.GetUserByEmailAsync("admin@marketdz.com");
                if (existingUser != null)
                {
                    LogInfo("Admin user already exists, skipping creation");
                    return true;
                }

                // Create a new admin user
                var adminUser = new User
                {
                    Id = 1,
                    Email = "admin@marketdz.com",
                    DisplayName = "Admin User",
                    PasswordHash = PasswordHasher.HashPassword("Admin@123"),
                    IsEmailVerified = true,
                    EmailVerifiedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsAdmin = true
                };

                var result = await _firebaseService.CreateUserAsync(adminUser);
                if (result)
                {
                    LogInfo("Sample admin user created successfully");
                    return true;
                }
                else
                {
                    LogWarning("Failed to create sample admin user");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error creating sample admin user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Create sample categories in Firebase
        /// </summary>
        public async Task<bool> CreateSampleCategoriesAsync()
        {
            try
            {
                LogInfo("Creating sample categories...");

                var categories = new List<string>
                {
                    "For Sale",
                    "For Rent",
                    "Jobs",
                    "Services",
                    "Real Estate",
                    "Vehicles",
                    "Electronics",
                    "Furniture",
                    "Clothing",
                    "Other"
                };

                int success = 0;
                for (int i = 0; i < categories.Count; i++)
                {
                    try
                    {
                        var category = new
                        {
                            Id = i + 1,
                            Name = categories[i],
                            CreatedAt = DateTime.UtcNow
                        };

                        await _firebaseService.SetAsync($"categories/{i + 1}", category);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error creating category {categories[i]}: {ex.Message}");
                    }
                }

                LogInfo($"Category creation completed: {success}/{categories.Count} created successfully");
                return success > 0;
            }
            catch (Exception ex)
            {
                LogError($"Category creation failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Create a sample item in Firebase for testing
        /// </summary>
        public async Task<bool> CreateSampleItemAsync(int userId)
        {
            try
            {
                LogInfo("Creating sample item...");

                // Get the user
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    LogWarning($"User {userId} not found for sample item");
                    return false;
                }

                // Create a sample item
                var item = new Item
                {
                    Title = "Sample Item",
                    Description = "This is a sample item created during Firebase setup",
                    Price = 100,
                    Category = "Electronics",
                    PostedByUserId = userId,
                    PostedByUser = user,
                    ListedDate = DateTime.UtcNow,
                    Status = ItemStatus.Active
                };

                var itemId = await _firebaseService.CreateItemAsync(item);
                if (itemId > 0)
                {
                    LogInfo($"Sample item created with ID {itemId}");
                    return true;
                }
                else
                {
                    LogWarning("Failed to create sample item");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError($"Error creating sample item: {ex.Message}");
                return false;
            }
        }

        #region Logging Helpers

        private void LogInfo(string message)
        {
            var logMessage = $"[INFO] {DateTime.Now}: {message}";
            Debug.WriteLine(logMessage);
            _setupLog.AppendLine(logMessage);
        }

        private void LogWarning(string message)
        {
            var logMessage = $"[WARNING] {DateTime.Now}: {message}";
            Debug.WriteLine(logMessage);
            _setupLog.AppendLine(logMessage);
        }

        private void LogError(string message)
        {
            var logMessage = $"[ERROR] {DateTime.Now}: {message}";
            Debug.WriteLine(logMessage);
            _setupLog.AppendLine(logMessage);
        }

        #endregion
    }
}