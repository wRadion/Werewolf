using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using Werewolf.Events.ClientRoomServerEvents;
using Werewolf.Views;

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

        private Socket _client;
        private BinaryWriter _writer;
        private BinaryReader _reader;
        private bool _isClosing;

        public string Name { get; private set; }
        public bool IsHost { get; private set; }
        public string IPAddressString { get; private set; }

        public event EventHandler<RoomUserListSetEventArgs> RoomUserListSet;
        public event EventHandler<RoomUserMessageSentEventArgs> RoomUserMessageSent;
        public event EventHandler<RoomUserJoinedEventArgs> RoomUserJoined;
        public event EventHandler<RoomUserLeftEventArgs> RoomUserLeft;

        private ClientRoom()
        {
            Reset();
        }

        private void Reset()
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _writer = null;
            _reader = null;

            Name = string.Empty;
            IsHost = false;
            IPAddressString = "<Not connected>";
        }

        public bool Connect(string name, IPAddress ipAddress)
        {
            if (_client.Connected) return true;

            _client.Connect(ipAddress, ServerRoom.DEFAULT_PORT);
            NetworkStream stream = new NetworkStream(_client);
            _writer = new BinaryWriter(stream);
            _reader = new BinaryReader(stream);

            Name = name;
            _writer.Write(Name);

            IsHost = _reader.ReadBoolean();
            IPAddressString = ipAddress.ToString();

            return _reader.ReadBoolean();
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
                finally
                {
                    if (!_isClosing)
                    {
                        MessageBox.Show("Une erreur est survenue : le serveur n'est plus accessible.", "Erreur - Serveur inaccessible", MessageBoxButton.OK, MessageBoxImage.Error);
                        Disconnect();
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ((MainWindow)Application.Current.MainWindow).SetView<MainView>();
                        });
                    }
                }
            });
        }

        public void Disconnect(bool isClosing = false)
        {
            _isClosing = isClosing;

            if (_reader != null) _reader.Close();
            if (_writer != null) _writer.Close();

            if (_client.Connected)
            {
                try
                {
                    _client.Shutdown(SocketShutdown.Both);
                }
                catch { }
                finally
                {
                    _client.Close();
                }
            }

            if (!isClosing)
                Reset();
        }
    }
}
