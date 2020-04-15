using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Werewolf.Network.Events;
using Werewolf.Network.Exceptions;

namespace Werewolf.Network
{
    public class Server
    {
        #region Singleton
        private static Server _instance = null;
        public static Server Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Server();
                return _instance;
            }
        }
        #endregion Singleton

        public const int DEFAULT_PORT = 9998;

        private readonly Socket _server;
        private readonly List<User> _users;
        private readonly EventManager<ClientToServerEventArgs> _userEvents;

        public bool Started { get; private set; }
        public EventManager<ServerEventArgs> ServerEvents { get; }

        private Server()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _users = new List<User>();
            _userEvents = new EventManager<ClientToServerEventArgs>();
            ServerEvents = new EventManager<ServerEventArgs>();

            _userEvents.AddListener<SendChatMessageEventArgs>((sender, e) =>
            {
                SendEvent(new ChatMessageSentEventArgs(((User)sender).Name, e.Message));
            });

            ServerEvents.AddListener<ServerUserConnectedEventArgs>((sender, e) =>
            {
                SendEvent(new UserJoinedEventArgs(e.User.Name));
                _users.Add(e.User);
                ListenUserEvents(e.User);
                e.User.SendEvent(new ChatMessageSentEventArgs(string.Empty, $"Bienvenue sur le salon, {e.User.Name} !"));
                e.User.SendEvent(new UserListSetEventArgs(_users.Select((u) => u.Name).ToArray()));
                Game.Game.Instance.AddPlayer(e.User);
            });

            ServerEvents.AddListener<ServerUserDisconnectedEventArgs>((sender, e) =>
            {
                _users.Remove(e.User);
                SendEvent(new UserLeftEventArgs(e.User.Name));
                Game.Game.Instance.RemovePlayer(e.User);
            });
        }

        public void Start(int port = DEFAULT_PORT)
        {
            if (_server.Connected) return;
            Started = true;

            _server.Bind(new IPEndPoint(IPAddress.Loopback, port));
            _server.Listen(10);
            ListenConnexions();
        }

        private void ListenConnexions()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        User user = null;

                        try
                        {
                            user = new User(_server.Accept(), _users.ToArray());
                            ServerEvents.RaiseEvent(this, new ServerUserConnectedEventArgs((user)));
                        }
                        catch (NameAlreadyTakenException)
                        {
                            user.Disconnect();
                        }
                    }
                }
                catch (Exception e) when (
                    e is SocketException ||
                    e is ObjectDisposedException)
                {
                    Utils.MessageBox.ShowException(e);
                }
            });
        }

        private void ListenUserEvents(User user)
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        _userEvents.RaiseEvent(user, user.ExpectEvent());
                    }
                }
                catch (Exception e) when (
                    e is SerializationException ||
                    e is EndOfStreamException ||
                    e is ObjectDisposedException ||
                    e is IOException)
                {
                    Utils.MessageBox.ShowException(e);
                }
                finally
                {
                    if (!user.IsHost)
                        ServerEvents.RaiseEvent(new ServerUserDisconnectedEventArgs((user)));
                }

            });
        }

        public void SendEvent<TEventArgs>(TEventArgs args) where TEventArgs : ServerToClientEventArgs
        {
            foreach (User user in _users)
                user.SendEvent(args);
        }

        public void Stop(bool isClosing = false)
        {
            foreach (User user in _users)
                user.Disconnect();
            _users.Clear();

            if (!_server.Connected) return;

            _server.Disconnect(!isClosing);
        }
    }
}
