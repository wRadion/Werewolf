using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    [Serializable]
    public class RoomUserNameChangedEventArgs : ServerToClientEventArgs
    {
        public string OldName { get; }
        public string NewName { get; }

        public RoomUserNameChangedEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }
}
