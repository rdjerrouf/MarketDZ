using System;
using MarketDZ.Models;
using System.Diagnostics;


namespace MarketDZ.Services
{
    public class FirebaseUserSessionService : IUserSessionService
    {
        private readonly FirebaseService _firebaseService;
        private User? _currentUser;

        public FirebaseUserSessionService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        }

        public User? CurrentUser => _currentUser;

        public bool IsLoggedIn => _currentUser != null;

        public void SetCurrentUser(User? user)
        {
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));
        }

        public void ClearCurrentUser()
        {
            _currentUser = null;
            // Clear the stored user ID from secure storage
            Task.Run(async () =>
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
                {
                    await SecureStorage.SetAsync("userId", string.Empty);
                }
                else
                {
                    Debug.WriteLine("SecureStorage.SetAsync is not supported on this platform.");
                }
            });
        }

        public async Task SaveSessionAsync()
        {
            if (_currentUser != null)
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
                {
                    await SecureStorage.SetAsync("userId", _currentUser.Id.ToString());
                    Debug.WriteLine($"Saved user ID {_currentUser.Id} to secure storage");
                }
                else
                {
                    Debug.WriteLine("SecureStorage.SetAsync is not supported on this platform.");
                }
            }
        }

        public async Task<bool> RestoreSessionAsync()
        {
            try
            {
                string? storedUserId = await SecureStorage.GetAsync("userId");

                if (string.IsNullOrEmpty(storedUserId))
                {
                    Debug.WriteLine("No stored user ID found");
                    return false;
                }

                if (!int.TryParse(storedUserId, out int userId))
                {
                    Debug.WriteLine("Stored user ID is invalid");
                    await SecureStorage.SetAsync("userId", string.Empty);
                    return false;
                }

                var user = await _firebaseService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    Debug.WriteLine($"User with ID {userId} not found in database");
                    await SecureStorage.SetAsync("userId", string.Empty);
                    return false;
                }

                _currentUser = user;
                Debug.WriteLine($"Successfully restored session for user: {user.Email}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restoring session: {ex.Message}");
                return false;
            }
        }
    }
}
