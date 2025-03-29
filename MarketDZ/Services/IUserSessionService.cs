// Services/IUserSessionService.cs
using MarketDZ.Models;
namespace MarketDZ.Services
{
    /// <summary>
    /// Manages user session state and persistence
    /// </summary>
    public interface IUserSessionService
    {
        User? CurrentUser { get; }
        bool IsLoggedIn { get; }
        void SetCurrentUser(User? user); // Change User to User?
        void ClearCurrentUser();
        Task SaveSessionAsync();
        Task<bool> RestoreSessionAsync();
    }
}
