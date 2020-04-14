using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class UserLeftEventArgs : ServerToClientEventArgs
    {
        public string Name { get; }

        public UserLeftEventArgs(string name)
        {
            Name = name;
        }
    }
}
