using System;
using System.IO;
using System.Net.Sockets;

namespace Werewolf.Classes.Room
{
    public class ServerRoomClient
    {
        private readonly Socket _client;

        public string Name;
        public bool IsHost;
        public readonly BinaryWriter Writer;
        public readonly BinaryReader Reader;

        public ServerRoomClient(Socket client, string tempName, bool isHost = false)
        {
            _client = client;
            NetworkStream stream = new NetworkStream(_client);
            Writer = new BinaryWriter(stream);
            Reader = new BinaryReader(stream);

            Name = tempName;
            IsHost = isHost;

            Writer.Write(tempName);
        }

        public void Send(ClientRoomServerEvent @event, params object[] args)
        {
            Writer.Write((int)@event);

            foreach (object obj in args)
            {
                if (obj is int i) Writer.Write(i);
                else if (obj is bool b) Writer.Write(b);
                else if (obj is string s) Writer.Write(s);
                else
                    throw new NotImplementedException();
            }
        }

        public void Disconnect()
        {
            Reader.Close();
            Writer.Close();
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }
    }
}
