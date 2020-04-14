using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class UserJoinedEventArgs : ServerToClientEventArgs
    {
        public string Name { get; }

        public UserJoinedEventArgs(string name)
        {
            Name = name;
        }
    }
}
