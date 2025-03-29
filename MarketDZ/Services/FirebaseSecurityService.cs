// Services/FirebaseSecurityService.cs
using MarketDZ.Models;
using System.Diagnostics;

namespace MarketDZ.Services
{
    public class FirebaseSecurityService
    {
        private readonly FirebaseService _firebaseService;

        public FirebaseSecurityService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        }

        // Reporting functionality
        public async Task<Report> ReportItemAsync(int itemId, int reportedByUserId, string reason, string? additionalComments = null)
        {
            try
            {
                // Check if the item exists
                var item = await _firebaseService.GetItemByIdAsync(itemId);
                if (item == null)
                    throw new Exception("Item not found");

                // Check if the user exists
                var user = await _firebaseService.GetUserByIdAsync(reportedByUserId);
                if (user == null)
                    throw new Exception("User not found");

                // Create the report
                var report = new Report
                {
                    Id = await GetNextReportIdAsync(),
                    ReportedItemId = itemId,
                    ReportedByUserId = reportedByUserId,
                    Reason = reason,
                    AdditionalComments = additionalComments,
                    ReportedAt = DateTime.UtcNow,
                    Status = ReportStatus.Pending
                };

                // Add to Firebase
                var success = await _firebaseService.SetAsync($"reports/{report.Id}", report);
                if (success == null)
                    throw new Exception("Failed to create report");

                return report;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reporting item: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Report>> GetUserReportsAsync(int userId)
        {
            try
            {
                var allReports = await GetAllReportsAsync();
                return allReports
                    .Where(r => r.ReportedByUserId == userId)
                    .OrderByDescending(r => r.ReportedAt)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user reports: {ex.Message}");
                return new List<Report>();
            }
        }

        public async Task<bool> HasUserReportedItemAsync(int userId, int itemId)
        {
            try
            {
                var allReports = await GetAllReportsAsync();
                return allReports.Any(r => r.ReportedByUserId == userId && r.ReportedItemId == itemId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking if user reported item: {ex.Message}");
                return false;
            }
        }

        // Blocking functionality
        public async Task<BlockedUser> BlockUserAsync(int userId, int blockedUserId, string? reason = null)
        {
            try
            {
                // Check if the users exist
                var user = await _firebaseService.GetUserByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found");

                var blockedUser = await _firebaseService.GetUserByIdAsync(blockedUserId);
                if (blockedUser == null)
                    throw new Exception("User to block not found");

                // Prevent blocking yourself
                if (userId == blockedUserId)
                    throw new Exception("You cannot block yourself");

                // Check if already blocked
                var isBlocked = await IsUserBlockedAsync(userId, blockedUserId);
                if (isBlocked)
                {
                    // Return the existing block record
                    var allBlocks = await GetAllBlockedUsersAsync();
                    return allBlocks.FirstOrDefault(b => b.UserId == userId && b.BlockedUserId == blockedUserId);
                }

                // Create the block record
                var blockId = await GetNextBlockedUserIdAsync();
                var block = new BlockedUser
                {
                    Id = blockId,
                    UserId = userId,
                    BlockedUserId = blockedUserId,
                    BlockedAt = DateTime.UtcNow,
                    Reason = reason
                };

                // Add to Firebase
                var success = await _firebaseService.SetAsync($"blockedUsers/{blockId}", block);
                if (success == null)
                    throw new Exception("Failed to block user");

                return block;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error blocking user: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UnblockUserAsync(int userId, int blockedUserId)
        {
            try
            {
                var allBlocks = await GetAllBlockedUsersAsync();
                var block = allBlocks.FirstOrDefault(b => b.UserId == userId && b.BlockedUserId == blockedUserId);

                if (block == null)
                    return false; // Not blocked

                await _firebaseService.DeleteAsync($"blockedUsers/{block.Id}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error unblocking user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> IsUserBlockedAsync(int userId, int blockedUserId)
        {
            try
            {
                var allBlocks = await GetAllBlockedUsersAsync();
                return allBlocks.Any(b => b.UserId == userId && b.BlockedUserId == blockedUserId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking if user is blocked: {ex.Message}");
                return false;
            }
        }

        public async Task<List<User>> GetBlockedUsersAsync(int userId)
        {
            try
            {
                var blockedUsers = new List<User>();
                var allBlocks = await GetAllBlockedUsersAsync();

                var userBlocks = allBlocks.Where(b => b.UserId == userId).ToList();

                foreach (var block in userBlocks)
                {
                    var blockedUser = await _firebaseService.GetUserByIdAsync(block.BlockedUserId);
                    if (blockedUser != null)
                    {
                        blockedUsers.Add(blockedUser);
                    }
                }

                return blockedUsers;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting blocked users: {ex.Message}");
                return new List<User>();
            }
        }

        #region Helper Methods

        private async Task<List<Report>> GetAllReportsAsync()
        {
            try
            {
                var reportCollection = await _firebaseService.GetCollectionAsync<Report>("reports");
                return reportCollection?.Select(r => r.Object).ToList() ?? new List<Report>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting all reports: {ex.Message}");
                return new List<Report>();
            }
        }

        private async Task<List<BlockedUser>> GetAllBlockedUsersAsync()
        {
            try
            {
                var blockedUsersCollection = await _firebaseService.GetCollectionAsync<BlockedUser>("blockedUsers");
                return blockedUsersCollection?.Select(b => b.Object).ToList() ?? new List<BlockedUser>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting all blocked users: {ex.Message}");
                return new List<BlockedUser>();
            }
        }

        private async Task<int> GetNextReportIdAsync()
        {
            try
            {
                var reports = await GetAllReportsAsync();
                return reports.Any() ? reports.Max(r => r.Id) + 1 : 1;
            }
            catch
            {
                return 1;
            }
        }

        private async Task<int> GetNextBlockedUserIdAsync()
        {
            try
            {
                var blockedUsers = await GetAllBlockedUsersAsync();
                return blockedUsers.Any() ? blockedUsers.Max(b => b.Id) + 1 : 1;
            }
            catch
            {
                return 1;
            }
        }

        #endregion
    }
}