using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    public class RoomUserNameChangedEventArgs : EventArgs
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
