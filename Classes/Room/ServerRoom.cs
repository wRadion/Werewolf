using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Werewolf.Classes.Room
{
    public enum ServerRoomClientEvent
    {
        CHANGE_NAME = 0,
        CHAT_MESSAGE = 1,
        JOIN_ROOM = 2,
        LEAVE_ROOM = 3
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
                        ServerRoomClient user = new ServerRoomClient(_server.Accept(), $"User{ CurrentId }", CurrentId++ == 0);
                        _users.Add(user);

                        Task.Run(() =>
                        {
                            try
                            {
                                while (true)
                                {
                                    ServerRoomClientEvent e = (ServerRoomClientEvent)user.Reader.ReadInt32();
                                    // call listeners
                                }
                            }
                            catch (EndOfStreamException) { }
                            catch (ObjectDisposedException) { }
                            catch (IOException) { }
                        });
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
