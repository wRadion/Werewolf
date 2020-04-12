using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    public class RoomUserJoinedEventArgs : EventArgs
    {
        public string Name { get; }

        public RoomUserJoinedEventArgs(string name)
        {
            Name = name;
        }
    }
}
