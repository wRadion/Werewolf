using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    [Serializable]
    public class RoomUserLeftEventArgs : ServerToClientEventArgs
    {
        public string Name { get; }

        public RoomUserLeftEventArgs(string name)
        {
            Name = name;
        }
    }
}
