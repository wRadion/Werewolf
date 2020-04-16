using System;

namespace Werewolf.Network.Events
{
    [Serializable]
    public class TimerUpdatedEventArgs : ServerToClientEventArgs
    {
        public int Seconds { get; }

        public TimerUpdatedEventArgs(int seconds)
        {
            Seconds = seconds;
        }
    }
}
