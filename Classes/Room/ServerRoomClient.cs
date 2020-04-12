using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Werewolf.Classes.Room
{
    public class ServerRoomClient
    {
        private readonly Socket _client;

        public int Id { get; }
        public string Name { get; set; }
        public bool IsHost;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;

        public ServerRoomClient(Socket client, int id, bool isHost = false)
        {
            _client = client;
            NetworkStream stream = new NetworkStream(_client);
            _writer = new BinaryWriter(stream);
            _reader = new BinaryReader(stream);

            Id = id;
            Name = _reader.ReadString();
            IsHost = isHost;
            _writer.Write(IsHost);
        }

        public void SendBoolean(bool b)
        {
            _writer.Write(b);
        }

        public void Send(ClientRoomServerEvent @event, params object[] args)
        {
            _writer.Write((int)@event);

            foreach (object obj in args)
            {
                if (obj is int i) _writer.Write(i);
                else if (obj is bool b) _writer.Write(b);
                else if (obj is string s) _writer.Write(s);
                else if (obj is object[] objs)
                {
                    foreach (object obj2 in objs)
                    {
                        if (obj2 is int i2) _writer.Write(i2);
                        else if (obj2 is bool b2) _writer.Write(b2);
                        else if (obj2 is string s2) _writer.Write(s2);
                        else
                            throw new NotImplementedException();
                    }
                }
                else
                    throw new NotImplementedException();
            }
        }

        public void Listen(ServerRoom server)
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        ServerRoomClientEvent e = (ServerRoomClientEvent)_reader.ReadInt32();

                        switch (e)
                        {
                            case ServerRoomClientEvent.ROOM_USER_SEND_MESSAGE:
                                string message = _reader.ReadString();
                                server.OnRoomUserSendMessage(this, message);
                                break;
                        }
                    }
                }
                catch (EndOfStreamException) { }
                catch (ObjectDisposedException) { }
                catch (IOException) { }
                finally
                {
                    if (!IsHost)
                        server.OnRoomUserLeft(this);
                }
            });
        }
        public void Disconnect()
        {
            _reader.Close();
            _writer.Close();

            _client.Disconnect(false);
        }
    }
}
