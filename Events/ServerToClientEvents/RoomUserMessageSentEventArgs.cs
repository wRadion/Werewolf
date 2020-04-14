using System;

namespace Werewolf.Events.ClientRoomServerEvents
{
    [Serializable]
    public class RoomUserMessageSentEventArgs : ServerToClientEventArgs
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
