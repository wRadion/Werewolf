using System;
using System.Linq;
using System.Net.Sockets;

using Werewolf.Network.Exceptions;
using Werewolf.Network.Packets;

namespace Werewolf.Network
{
    public class User : IEquatable<User>
    {
        private readonly Socket _client;
        private readonly PacketManager _packets;

        public string Name { get; set; }
        public bool IsHost { get; }

        public User(Socket client, User[] users)
        {
            _client = client;
            _packets = new PacketManager(new NetworkStream(_client));

            Packet<string> packetName = _packets.Expect<Packet<string>>();

            Name = packetName.Data1;
            IsHost = users.Length == 0;
            bool isNameTaken = users.Any((user) => Equals(user));

            _packets.Send(new Packet<bool>(isNameTaken));

            if (isNameTaken)
                throw new NameAlreadyTakenException(Name);
        }

        public void SendEvent<TEventArgs>(TEventArgs args) where TEventArgs : EventArgs
        {
            _packets.Send(new PacketEvent(args));
        }

        public dynamic ExpectEvent()
        {
            return _packets.Expect<PacketEvent>().EventArgs;
        }

        public void Disconnect()
        {
            _packets.Close();
            _client.Disconnect(false);
        }

        public bool Equals(User other) => Name.Equals(other.Name);
    }
}
