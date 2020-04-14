using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Werewolf.Events;
using Werewolf.Events.ClientRoomServerEvents;
using Werewolf.Events.ClientToServerEvents;
using Werewolf.Events.ServerEvents;
using Werewolf.Network;
using Werewolf.Network.Exceptions;

namespace Werewolf.Models.Room
{
    public class ServerRoom
    {
        #region Singleton
        private static ServerRoom _instance = null;
        public static ServerRoom Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ServerRoom();
                return _instance;
            }
        }
        #endregion Singleton

        public const int DEFAULT_PORT = 9998;

        private readonly Socket _server;
        private readonly List<ServerRoomClient> _users;
        private readonly EventManager<ClientToServerEventArgs> _userEvents;

        public EventManager<ServerEventArgs> ServerEvents { get; }

        private ServerRoom()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _users = new List<ServerRoomClient>();
            _userEvents = new EventManager<ClientToServerEventArgs>();
            ServerEvents = new EventManager<ServerEventArgs>();

            _userEvents.AddListener<SendChatMessageEventArgs>((sender, e) =>
            {
                SendEvent(new RoomUserMessageSentEventArgs(((ServerRoomClient)sender).Name, e.Message));
            });

            ServerEvents.AddListener<ServerUserConnectedEventArgs>((sender, e) =>
            {
                SendEvent(new RoomUserJoinedEventArgs(e.User.Name));
                _users.Add(e.User);
                ListenUserEvents(e.User);
                e.User.SendEvent(new RoomUserMessageSentEventArgs(string.Empty, $"Bienvenue sur le salon, {e.User.Name} !"));
                e.User.SendEvent(new RoomUserListSetEventArgs(_users.Select((u) => u.Name).ToArray()));
            });

            ServerEvents.AddListener<ServerUserDisconnectedEventArgs>((sender, e) =>
            {
                _users.Remove(e.User);
                SendEvent(new RoomUserLeftEventArgs(e.User.Name));
            });
        }

        public void Start(int port = DEFAULT_PORT)
        {
            if (_server.Connected) return;

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
                        ServerRoomClient user = null;

                        try
                        {
                            user = new ServerRoomClient(_server.Accept(), _users.ToArray());
                            ServerEvents.RaiseEvent(this, new ServerUserConnectedEventArgs((user)));
                        }
                        catch (NameAlreadyTakenException)
                        {
                            user.Disconnect();
                        }
                    }
                }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }
            });
        }

        private void ListenUserEvents(ServerRoomClient user)
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
                catch (SerializationException) { }
                catch (EndOfStreamException) { }
                catch (ObjectDisposedException) { }
                catch (IOException) { }
                finally
                {
                    if (!user.IsHost)
                        ServerEvents.RaiseEvent(new ServerUserDisconnectedEventArgs((user)));
                }

            });
        }

        public void SendEvent<TEventArgs>(TEventArgs args) where TEventArgs : ServerToClientEventArgs
        {
            foreach (ServerRoomClient user in _users)
                user.SendEvent(args);
        }

        public void Stop(bool isClosing = false)
        {
            foreach (ServerRoomClient user in _users)
                user.Disconnect();
            _users.Clear();

            if (!_server.Connected) return;

            _server.Disconnect(!isClosing);
        }
    }
}
