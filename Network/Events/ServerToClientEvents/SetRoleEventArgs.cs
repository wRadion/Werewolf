using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class SetRoleEventArgs : ServerToClientEventArgs
    {
        public int RoleId { get; }

        public SetRoleEventArgs(int roleId)
        {
            RoleId = roleId;
        }
    }
}
