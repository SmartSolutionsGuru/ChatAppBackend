using System.Collections.Concurrent;

namespace ChatApp.API.Presence
{
    public class UserConnectionTracker
    {
        private readonly ConcurrentDictionary<string, int> _connections = new();

        public bool UserConnected(string userId)
        {
            var count = _connections.AddOrUpdate(userId, 1, (_, c) => c + 1);
            return count == 1; // first connection
        }

        public bool UserDisconnected(string userId)
        {
            if (!_connections.TryGetValue(userId, out var count))
                return false;

            if (count <= 1)
            {
                _connections.TryRemove(userId, out _);
                return true; // last connection closed
            }

            _connections[userId] = count - 1;
            return false;
        }

        public bool IsOnline(string userId)
            => _connections.ContainsKey(userId);

        public IEnumerable<string> GetOnlineUserIds()
            => _connections.Keys.ToList();
    }
}
