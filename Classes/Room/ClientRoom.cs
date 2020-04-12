using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Werewolf.Events.ClientRoomServerEvents;

namespace Werewolf.Classes.Room
{
    public enum ClientRoomServerEvent
    {
        ROOM_USER_LIST_SET = 0,
        ROOM_USER_MESSAGE_SENT = 1,
        ROOM_USER_JOINED = 2,
        ROOM_USER_LEFT = 3
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

        public event EventHandler<RoomUserListSetEventArgs> RoomUserListSet;
        public event EventHandler<RoomUserMessageSentEventArgs> RoomUserMessageSent;
        public event EventHandler<RoomUserJoinedEventArgs> RoomUserJoined;
        public event EventHandler<RoomUserLeftEventArgs> RoomUserLeft;

        private ClientRoom()
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _writer = null;
            _reader = null;

            Name = string.Empty;
            IPAddressString = "<Not connected>";
        }

        public void Connect(string name, string ipAddressString)
        {
            if (_client.Connected) return;

            IPAddress ipAddress = ipAddressString == null ? IPAddress.Loopback : IPAddress.Parse(ipAddressString);

            _client.Connect(new IPEndPoint(ipAddress, ServerRoom.DEFAULT_PORT));
            NetworkStream stream = new NetworkStream(_client);
            _writer = new BinaryWriter(stream);
            _reader = new BinaryReader(stream);

            Name = name;
            IPAddressString = ipAddress.ToString();

            _writer.Write(Name);
        }

        public void Send(ServerRoomClientEvent @event, params object[] args)
        {
            _writer.Write((int)@event);

            foreach (object obj in args)
            {
                if (obj is int i) _writer.Write(i);
                else if (obj is bool b) _writer.Write(b);
                else if (obj is string s) _writer.Write(s);
                else
                    throw new NotImplementedException();
            }
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

                        switch (e)
                        {
                            case ClientRoomServerEvent.ROOM_USER_LIST_SET:
                                int length = _reader.ReadInt32();
                                string[] userList = new string[length];
                                for (int i = 0; i < length; ++i)
                                    userList[i] = _reader.ReadString();
                                RoomUserListSet?.Invoke(this, new RoomUserListSetEventArgs(userList));
                                break;
                            case ClientRoomServerEvent.ROOM_USER_MESSAGE_SENT:
                                string name = _reader.ReadString();
                                string message = _reader.ReadString();
                                RoomUserMessageSent?.Invoke(this, new RoomUserMessageSentEventArgs(name, message));
                                break;
                            case ClientRoomServerEvent.ROOM_USER_JOINED:
                                string userJoinedName = _reader.ReadString();
                                RoomUserJoined?.Invoke(this, new RoomUserJoinedEventArgs(userJoinedName));
                                break;
                            case ClientRoomServerEvent.ROOM_USER_LEFT:
                                string userLeftName = _reader.ReadString();
                                RoomUserLeft?.Invoke(this, new RoomUserLeftEventArgs(userLeftName));
                                break;
                        }
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
