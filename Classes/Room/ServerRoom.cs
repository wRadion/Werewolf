using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Werewolf.Classes.Room
{
    public enum ServerRoomClientEvent
    {
        ROOM_USER_SEND_MESSAGE = 0
    }

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
        private static int CurrentId = 0;

        private readonly Socket _server;
        private readonly List<ServerRoomClient> _users;

        private ServerRoom()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _users = new List<ServerRoomClient>();
        }

        public void OnRoomUserSendMessage(ServerRoomClient sender, string message)
        {
            Send(ClientRoomServerEvent.ROOM_USER_MESSAGE_SENT, sender.Name, message);
        }

        public void OnRoomUserJoined(ServerRoomClient sender)
        {
            Send(ClientRoomServerEvent.ROOM_USER_JOINED, sender.Name);
            _users.Add(sender);
            sender.Listen(this);
            sender.Send(ClientRoomServerEvent.ROOM_USER_MESSAGE_SENT, string.Empty, $"Bienvenue sur le salon, {sender.Name} !");
            sender.Send(ClientRoomServerEvent.ROOM_USER_LIST_SET, _users.Count, _users.Select((u) => u.Name).ToArray());
        }

        public void OnRoomUserLeft(ServerRoomClient sender)
        {
            _users.Remove(sender);
            Send(ClientRoomServerEvent.ROOM_USER_LEFT, sender.Name);
        }

        public void Send(ClientRoomServerEvent @event, params object[] args)
        {
            foreach (ServerRoomClient user in _users)
                user.Send(@event, args);
        }

        public void Start(int port = DEFAULT_PORT)
        {
            if (_server.Connected) return;

            _server.Bind(new IPEndPoint(IPAddress.Loopback, port));
            _server.Listen(10);

            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        ServerRoomClient user = new ServerRoomClient(_server.Accept(), CurrentId, CurrentId++ == 0);
                        OnRoomUserJoined(user);
                    }
                }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }
            });
        }

        public void Stop()
        {
            if (!_server.Connected) return;

            foreach (ServerRoomClient user in _users)
                user.Disconnect();
            _users.Clear();

            _server.Shutdown(SocketShutdown.Both);
            _server.Close();
        }
    }
}
