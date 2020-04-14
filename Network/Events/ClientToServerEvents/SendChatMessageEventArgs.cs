using System;

namespace Werewolf.Network.Events
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
