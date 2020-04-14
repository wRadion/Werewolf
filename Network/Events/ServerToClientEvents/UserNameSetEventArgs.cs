using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class UserNameSetEventArgs : ServerToClientEventArgs
    {
        public string TemporaryName { get; }

        public UserNameSetEventArgs(string temporaryName)
        {
            TemporaryName = temporaryName;
        }
    }
}
