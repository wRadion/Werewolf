using System;

using Werewolf.Models.Room;

namespace Werewolf.Events.ServerEvents
{
    [Serializable]
    public class ServerUserDisconnectedEventArgs : ServerEventArgs
    {
        public ServerRoomClient User { get; }

        public ServerUserDisconnectedEventArgs(ServerRoomClient user)
        {
            User = user;
        }
    }
}
