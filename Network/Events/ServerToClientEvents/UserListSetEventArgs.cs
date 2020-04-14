using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class UserListSetEventArgs : ServerToClientEventArgs
    {
        public string[] UserList { get; }

        public UserListSetEventArgs(string[] userList)
        {
            UserList = userList;
        }
    }
}
