using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    public class RoomUserListSetEventArgs : EventArgs
    {
        public string[] UserList { get; }

        public RoomUserListSetEventArgs(string[] userList)
        {
            UserList = userList;
        }
    }
}
