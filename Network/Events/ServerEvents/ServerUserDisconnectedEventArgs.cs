using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class ServerUserDisconnectedEventArgs : ServerEventArgs
    {
        public User User { get; }

        public ServerUserDisconnectedEventArgs(User user)
        {
            User = user;
        }
    }
}
