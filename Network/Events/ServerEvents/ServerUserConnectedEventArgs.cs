using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class ServerUserConnectedEventArgs : ServerEventArgs
    {
        public User User { get; }

        public ServerUserConnectedEventArgs(User user)
        {
            User = user;
        }
    }
}
