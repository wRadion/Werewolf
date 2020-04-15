using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class GameStartedEventArgs : ServerToClientEventArgs
    {
        public int[] RoleIds { get; }

        public GameStartedEventArgs(int[] roleIds)
        {
            RoleIds = roleIds;
        }
    }
}
