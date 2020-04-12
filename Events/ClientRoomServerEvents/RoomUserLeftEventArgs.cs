using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    public class RoomUserLeftEventArgs : EventArgs
    {
        public string Name { get; }

        public RoomUserLeftEventArgs(string name)
        {
            Name = name;
        }
    }
}
