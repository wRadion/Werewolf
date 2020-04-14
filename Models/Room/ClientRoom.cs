using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;

using Werewolf.Events;
using Werewolf.Events.ClientRoomServerEvents;
using Werewolf.Events.ClientToServerEvents;
using Werewolf.Network;
using Werewolf.Views;

namespace Werewolf.Models.Room
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
        private PacketManager _packets;
        private bool _isClosing;

        public string Name { get; private set; }
        public bool IsHost { get; private set; }
        public string IPAddressString { get; private set; }
        public EventManager<ServerToClientEventArgs> ServerEvents { get; private set; }

        private ClientRoom()
        {
            Reset();
        }

        private void Reset()
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _packets = null;

            Name = string.Empty;
            IsHost = false;
            IPAddressString = "<Not connected>";
            ServerEvents = new EventManager<ServerToClientEventArgs>();
        }

        public bool Connect(string name, IPAddress ipAddress)
        {
            if (_client.Connected) return true;

            _client.Connect(ipAddress, ServerRoom.DEFAULT_PORT);
            _packets = new PacketManager(new NetworkStream(_client));

            _packets.Send(new Packet<string>(name));
            Packet<bool> packetIsHost = _packets.Expect<Packet<bool>>();
            Packet<bool> packetIsNameTaken = _packets.Expect<Packet<bool>>();

            Name = name;
            IsHost = packetIsHost.Data1;
            IPAddressString = ipAddress.ToString();

            return !packetIsNameTaken.Data1;
        }

        public void ListenServerEvents()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        ServerEvents.RaiseEvent(ExpectEvent());
                    }
                }
                catch (SerializationException) { }
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

        public void SendEvent<TEventArgs>(TEventArgs args) where TEventArgs : ClientToServerEventArgs
        {
            _packets.Send(new PacketEvent(args));
        }

        public dynamic ExpectEvent()
        {
            return _packets.Expect<PacketEvent>().EventArgs;
        }

        public void Disconnect(bool isClosing = false)
        {
            _isClosing = isClosing;
            _packets.Close();

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
