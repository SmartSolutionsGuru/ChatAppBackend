namespace ChatApp.Application.Interfaces;

/// <summary>
/// Tracks user connection state for online/offline presence
/// </summary>
public interface IUserConnectionTracker
{
    /// <summary>
    /// Records a new connection for a user
    /// </summary>
    /// <returns>True if this is the user's first connection (they just came online)</returns>
    bool UserConnected(string userId);

    /// <summary>
    /// Records a disconnection for a user
    /// </summary>
    /// <returns>True if this was the user's last connection (they are now offline)</returns>
    bool UserDisconnected(string userId);

    /// <summary>
    /// Checks if a user is currently online
    /// </summary>
    bool IsOnline(string userId);

    /// <summary>
    /// Gets all currently online user IDs
    /// </summary>
    IEnumerable<string> GetOnlineUserIds();
}
