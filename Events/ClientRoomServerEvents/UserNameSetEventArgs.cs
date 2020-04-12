using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    public class UserNameSetEventArgs : EventArgs
    {
        public string TemporaryName { get; }

        public UserNameSetEventArgs(string temporaryName)
        {
            TemporaryName = temporaryName;
        }
    }
}
