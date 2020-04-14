using System;

namespace Werewolf.Events.ClientToServerEvents
{
    [Serializable]
    public class SendChatMessageEventArgs : ClientToServerEventArgs
    {
        public string Message { get; }

        public SendChatMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
