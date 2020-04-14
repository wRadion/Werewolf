using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    [Serializable]
    public class RoomUserListSetEventArgs : ServerToClientEventArgs
    {
        public string[] UserList { get; }

        public RoomUserListSetEventArgs(string[] userList)
        {
            UserList = userList;
        }
    }
}
