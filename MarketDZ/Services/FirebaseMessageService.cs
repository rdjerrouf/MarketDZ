using MarketDZ.Models;
using System.Diagnostics;

namespace MarketDZ.Services
{
    /// <summary>
    /// Implements message-related operations using Firebase
    /// </summary>
    public class FirebaseMessageService : IMessageService
    {
        private readonly FirebaseService _firebaseService;

        /// <summary>
        /// Constructor that injects required services
        /// </summary>
        public FirebaseMessageService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        }

        /// <summary>
        /// Retrieves all inbox messages for a specific user
        /// </summary>
        public async Task<IEnumerable<Message>> GetUserInboxMessagesAsync(int userId)
        {
            try
            {
                Debug.WriteLine($"GetUserInboxMessagesAsync called for user {userId}");

                // Get all messages
                var messages = await _firebaseService.GetMessagesForUserAsync(userId);

                // Filter to show only received messages
                var inboxMessages = messages.Where(m => m.ReceiverId == userId)
                    .OrderByDescending(m => m.Timestamp)
                    .ToList();

                Debug.WriteLine($"Retrieved {inboxMessages.Count()} messages for user {userId}");
                return inboxMessages;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving user inbox messages: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return Enumerable.Empty<Message>();
            }
        }

        /// <summary>
        /// Sends a new message
        /// </summary>
        public async Task<bool> SendMessageAsync(Message message)
        {
            try
            {
                // Ensure timestamp is set to current UTC time
                message.Timestamp = DateTime.UtcNow;

                // Mark as unread by default
                message.IsRead = false;

                // Create the message in Firebase
                var messageId = await _firebaseService.CreateMessageAsync(message);

                return messageId > 0;
            }
            catch (Exception ex)
            {
                // Log sending errors
                Debug.WriteLine($"Error sending message: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Marks a specific message as read
        /// </summary>
        public async Task<bool> MarkMessageAsReadAsync(int messageId)
        {
            try
            {
                // Find the message
                var message = await _firebaseService.GetMessageByIdAsync(messageId);

                if (message == null)
                {
                    Debug.WriteLine($"Message with ID {messageId} not found");
                    return false;
                }

                // Mark as read
                message.IsRead = true;
                return await _firebaseService.UpdateMessageAsync(message);
            }
            catch (Exception ex)
            {
                // Log marking errors
                Debug.WriteLine($"Error marking message as read: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a specific message
        /// </summary>
        public async Task<bool> DeleteMessageAsync(int messageId)
        {
            try
            {
                // Delete the message from Firebase
                await _firebaseService.DeleteAsync($"messages/{messageId}");
                return true;
            }
            catch (Exception ex)
            {
                // Log deletion errors
                Debug.WriteLine($"Error deleting message: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves a specific message by its ID
        /// </summary>
        public async Task<Message?> GetMessageByIdAsync(int messageId)
        {
            try
            {
                return await _firebaseService.GetMessageByIdAsync(messageId);
            }
            catch (Exception ex)
            {
                // Log retrieval errors
                Debug.WriteLine($"Error retrieving message: {ex.Message}");
                return null;
            }
        }
    }
}