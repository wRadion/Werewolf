using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Werewolf.Classes.Room
{
    public enum ClientRoomServerEvent
    {
        USER_NAME_SET = 0,
        ROOM_USER_NAME_CHANGED = 1,
        ROOM_USER_LIST_CHANGED = 2,
        ROOM_MESSAGE_SENT = 3,
        ROOM_USER_JOINED = 4,
        ROOM_USER_LEFT = 5
    }

    public class ClientRoom
    {
        #region Singleton
        private static ClientRoom _instance = null;
        public static ClientRoom Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ClientRoom();
                return _instance;
            }
        }
        #endregion Singleton

        private readonly Socket _client;
        private BinaryWriter _writer;
        private BinaryReader _reader;

        public string Name;
        public string IPAddressString;

        private ClientRoom()
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _writer = null;
            _reader = null;

            Name = string.Empty;
            IPAddressString = "<Not connected>";
        }

        public void Connect(string ipAddressString = null)
        {
            if (_client.Connected) return;

            IPAddress ipAddress = ipAddressString == null ? IPAddress.Loopback : IPAddress.Parse(ipAddressString);

            _client.Connect(new IPEndPoint(ipAddress, ServerRoom.DEFAULT_PORT));
            NetworkStream stream = new NetworkStream(_client);
            _writer = new BinaryWriter(stream);
            _reader = new BinaryReader(stream);

            IPAddressString = ipAddress.ToString();
        }

        public void Listen()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        ClientRoomServerEvent e = (ClientRoomServerEvent)_reader.ReadInt32();
                    }
                }
                catch (EndOfStreamException) { }
                catch (ObjectDisposedException) { }
                catch (IOException) { }
            });
        }

        public void Disconnect()
        {
            if (!_client.Connected) return;

            _reader.Close();
            _writer.Close();
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }
    }
}
