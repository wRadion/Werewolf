using System;

namespace Werewolf.Network.Events
{
    public abstract class Event { }

    public sealed class Event<TEventArgs> : Event where TEventArgs : EventArgs
    {
        public Type EventArgsType => typeof(TEventArgs);

        private event EventHandler<TEventArgs> Listeners;

        public Event() { }

        public void AddListener(EventHandler<TEventArgs> listener) => Listeners += listener;
        public void RaiseEvent(TEventArgs e) => Listeners?.Invoke(this, e);
        public void RaiseEvent(object sender, TEventArgs e) => Listeners?.Invoke(sender, e);
    }
}
