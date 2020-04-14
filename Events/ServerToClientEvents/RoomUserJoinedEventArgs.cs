using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    [Serializable]
    public class RoomUserJoinedEventArgs : ServerToClientEventArgs
    {
        public string Name { get; }

        public RoomUserJoinedEventArgs(string name)
        {
            Name = name;
        }
    }
}
