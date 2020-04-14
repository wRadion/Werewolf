using System;

using Werewolf.Models.Room;

namespace Werewolf.Events.ServerEvents
{
    [Serializable]
    public class ServerUserConnectedEventArgs : ServerEventArgs
    {
        public ServerRoomClient User { get; }

        public ServerUserConnectedEventArgs(ServerRoomClient user)
        {
            User = user;
        }
    }
}
