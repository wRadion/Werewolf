using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    public class RoomUserMessageSentEventArgs : EventArgs
    {
        public string Name { get; }
        public string Message { get; }

        public RoomUserMessageSentEventArgs(string name, string message)
        {
            Name = name;
            Message = message;
        }
    }
}
