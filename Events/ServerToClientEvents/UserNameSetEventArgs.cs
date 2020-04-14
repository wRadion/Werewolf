using System;

namespace Werewolf.Events.ClientRoomServerEvents
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
