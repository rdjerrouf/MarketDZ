using Firebase.Database;
using Firebase.Database.Query;
using MarketDZ.Models;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MarketDZ.Services
{
    /// <summary>
    /// Core service for interacting with Firebase Realtime Database
    /// </summary>
    public class FirebaseService
    {
        private readonly FirebaseClient _firebaseClient;
        private bool _isInitialized;
        private static SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public FirebaseService(string? firebaseUrl = null)
        {
            // Log the Firebase URL we're trying to connect to
            Debug.WriteLine($"Initializing Firebase with URL: {firebaseUrl ?? "null"}");

            // Initialize Firebase client with your Firebase URL
            _firebaseClient = new FirebaseClient(
                firebaseUrl ?? "https://marketdz-a6db7-default-rtdb.firebaseio.com/",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult<string?>(null)
                });
        }

        /// <summary>
        /// Initialize Firebase connection and verify access
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                await _initLock.WaitAsync();
                if (_isInitialized) return;

                Debug.WriteLine("Starting Firebase initialization");

                // Create a proper JSON object to send
                var testData = new { message = "Connection test: " + DateTime.UtcNow.ToString() };

                // Put the data as a properly formatted JSON object
                await _firebaseClient
                    .Child("test")
                    .PutAsync(testData);

                Debug.WriteLine("Firebase write test successful");

                // Try reading data as well
                var readResult = await _firebaseClient
                    .Child("test")
                    .OnceSingleAsync<object>();

                Debug.WriteLine($"Firebase read test result: {readResult != null}");
                Debug.WriteLine("Firebase connection successful");
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firebase initialization error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
            finally
            {
                _initLock.Release();
            }
        }

        #region Generic Firebase Operations

        /// <summary>
        /// Get a single object from Firebase at the specified path
        /// </summary>
        public async Task<T> GetAsync<T>(string path)
        {
            await InitializeAsync();
            return await _firebaseClient.Child(path).OnceSingleAsync<T>();
        }

        /// <summary>
        /// Get a collection of objects from Firebase at the specified path
        /// </summary>
        public async Task<IReadOnlyCollection<FirebaseObject<T>>> GetCollectionAsync<T>(string path)
        {
            await InitializeAsync();
            return await _firebaseClient.Child(path).OnceAsync<T>();
        }

        /// <summary>
        /// Save or update an object at the specified path
        /// </summary>
        public async Task<T> SetAsync<T>(string path, T data)
        {
            await InitializeAsync();
            await _firebaseClient.Child(path).PutAsync(data);
            return data;
        }

        /// <summary>
        /// Add a new object to a collection with an auto-generated key
        /// </summary>
        public async Task<FirebaseObject<T>> AddAsync<T>(string path, T data)
        {
            await InitializeAsync();
            return await _firebaseClient.Child(path).PostAsync(data);
        }

        /// <summary>
        /// Update specific properties of an object
        /// </summary>
        public async Task UpdateAsync<T>(string path, IDictionary<string, object> updates)
        {
            await InitializeAsync();
            foreach (var update in updates)
            {
                await _firebaseClient.Child(path).Child(update.Key).PutAsync(update.Value);
            }
        }

        /// <summary>
        /// Delete an object at the specified path
        /// </summary>
        public async Task DeleteAsync(string path)
        {
            await InitializeAsync();
            await _firebaseClient.Child(path).DeleteAsync();
        }

        #endregion

        #region User Operations

        /// <summary>
        /// Get a user by their unique ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                return await GetAsync<User>($"users/{userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user by ID: {ex.Message}");
                return null;
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
                // Firebase doesn't support direct querying by field value
                // So we need to retrieve all users and filter
                var users = await GetCollectionAsync<User>("users");
                return users
                    .Select(u => u.Object)
                    .FirstOrDefault(u => u.Email?.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user by email: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                if (user.Id <= 0)
                {
                    // Generate a new ID if not provided
                    user.Id = await GetNextUserIdAsync();
                }

                await SetAsync($"users/{user.Id}", user);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                await SetAsync($"users/{user.Id}", user);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get the next available user ID
        /// </summary>
        private async Task<int> GetNextUserIdAsync()
        {
            try
            {
                var users = await GetCollectionAsync<User>("users");
                if (!users.Any())
                    return 1;

                return users.Max(u => u.Object.Id) + 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion

        #region Item Operations

        /// <summary>
        /// Get an item by its unique ID
        /// </summary>
        public async Task<Item?> GetItemByIdAsync(int itemId)
        {
            try
            {
                return await GetAsync<Item>($"items/{itemId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting item by ID: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Get all items
        /// </summary>
        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            try
            {
                var items = await GetCollectionAsync<Item>("items");
                return items.Select(i => i.Object);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting items: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        /// <summary>
        /// Create a new item
        /// </summary>
        public async Task<int> CreateItemAsync(Item item)
        {
            try
            {
                if (item.Id <= 0)
                {
                    // Generate a new ID if not provided
                    item.Id = await GetNextItemIdAsync();
                }

                if (item.ListedDate == default)
                {
                    item.ListedDate = DateTime.UtcNow;
                }

                await SetAsync($"items/{item.Id}", item);
                return item.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating item: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Update an existing item
        /// </summary>
        public async Task<bool> UpdateItemAsync(Item item)
        {
            try
            {
                await SetAsync($"items/{item.Id}", item);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating item: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        public async Task<bool> DeleteItemAsync(int itemId)
        {
            try
            {
                await DeleteAsync($"items/{itemId}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting item: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all items posted by a specific user
        /// </summary>
        public async Task<IEnumerable<Item>> GetItemsByUserIdAsync(int userId)
        {
            try
            {
                var items = await GetItemsAsync();
                return items.Where(i => i.PostedByUserId == userId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting items by user ID: {ex.Message}");
                return Enumerable.Empty<Item>();
            }
        }

        /// <summary>
        /// Get the next available item ID
        /// </summary>
        private async Task<int> GetNextItemIdAsync()
        {
            try
            {
                var items = await GetCollectionAsync<Item>("items");
                if (!items.Any())
                    return 1;

                return items.Max(i => i.Object.Id) + 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion

        #region Message Operations

        /// <summary>
        /// Get a message by its unique ID
        /// </summary>
        public async Task<Message?> GetMessageByIdAsync(int messageId)
        {
            try
            {
                return await GetAsync<Message>($"messages/{messageId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting message by ID: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create a new message
        /// </summary>
        public async Task<int> CreateMessageAsync(Message message)
        {
            try
            {
                if (message.Id <= 0)
                {
                    // Generate a new ID if not provided
                    message.Id = await GetNextMessageIdAsync();
                }

                if (message.Timestamp == default)
                {
                    message.Timestamp = DateTime.UtcNow;
                }

                await SetAsync($"messages/{message.Id}", message);
                return message.Id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating message: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Update an existing message
        /// </summary>
        public async Task<bool> UpdateMessageAsync(Message message)
        {
            try
            {
                await SetAsync($"messages/{message.Id}", message);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating message: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get all messages for a specific user (sent or received)
        /// </summary>
        public async Task<IEnumerable<Message>> GetMessagesForUserAsync(int userId)
        {
            try
            {
                var messages = await GetCollectionAsync<Message>("messages");
                return messages
                    .Select(m => m.Object)
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .OrderByDescending(m => m.Timestamp);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting messages for user: {ex.Message}");
                return Enumerable.Empty<Message>();
            }
        }

        /// <summary>
        /// Get the next available message ID
        /// </summary>
        private async Task<int> GetNextMessageIdAsync()
        {
            try
            {
                var messages = await GetCollectionAsync<Message>("messages");
                if (!messages.Any())
                    return 1;

                return messages.Max(m => m.Object.Id) + 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion

        #region Verification Token Operations

        /// <summary>
        /// Create a verification token
        /// </summary>
        public async Task<bool> CreateVerificationTokenAsync(VerificationToken token)
        {
            try
            {
                if (token.Id <= 0)
                {
                    token.Id = await GetNextTokenIdAsync();
                }

                if (token.CreatedAt == default)
                {
                    token.CreatedAt = DateTime.UtcNow;
                }

                // Make sure we have the user object
                if (token.User == null && token.UserId > 0)
                {
                    var user = await GetUserByIdAsync(token.UserId);
                    if (user == null)
                    {
                        Debug.WriteLine($"User with ID {token.UserId} not found");
                        return false;
                    }
                    token.User = user; // Non-null assignment
                }

                await SetAsync($"verificationTokens/{token.Id}", token);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating verification token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get a verification token by its token string
        /// </summary>
        public async Task<VerificationToken?> GetVerificationTokenAsync(string token)
        {
            try
            {
                var tokens = await GetCollectionAsync<VerificationToken>("verificationTokens");
                var verificationToken = tokens
                    .Select(t => t.Object)
                    .FirstOrDefault(t => t.Token == token);

                if (verificationToken != null && verificationToken.User == null && verificationToken.UserId > 0)
                {
                    // Load the user if not already loaded
                    var user = await GetUserByIdAsync(verificationToken.UserId);
                    if (user != null)
                    {
                        verificationToken.User = user; // Non-null assignment
                    }
                    else
                    {
                        Debug.WriteLine($"User with ID {verificationToken.UserId} not found for token");
                        // If you can't return a token without a user, return null here
                        // Otherwise, keep the token without a User property set
                    }
                }

                return verificationToken; // Explicitly allowing null return
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting verification token: {ex.Message}");
                return null; // Explicitly allowing null return
            }
        }

        /// <summary>
        /// Get the next available token ID
        /// </summary>
        private async Task<int> GetNextTokenIdAsync()
        {
            try
            {
                var tokens = await GetCollectionAsync<VerificationToken>("verificationTokens");
                if (!tokens.Any())
                    return 1;

                return tokens.Max(t => t.Object.Id) + 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion
    }
}