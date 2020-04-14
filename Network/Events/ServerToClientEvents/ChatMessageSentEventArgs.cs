using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class ChatMessageSentEventArgs : ServerToClientEventArgs
    {
        public string Name { get; }
        public string Message { get; }

        public ChatMessageSentEventArgs(string name, string message)
        {
            Name = name;
            Message = message;
        }
    }
}
