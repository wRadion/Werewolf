﻿using System;
using System.Linq;
using System.Net.Sockets;

using Werewolf.Network;
using Werewolf.Network.Exceptions;

namespace Werewolf.Models.Room
{
    public class ServerRoomClient
    {
        private readonly Socket _client;
        private readonly PacketManager _packets;

        public string Name { get; set; }
        public bool IsHost;

        public ServerRoomClient(Socket client, ServerRoomClient[] users)
        {
            _client = client;
            _packets = new PacketManager(new NetworkStream(_client));

            IsHost = users.Length == 0;

            Packet<string> packetName = _packets.Expect<Packet<string>>();

            bool isNameTaken = users.Any((user) => user.Name == packetName.Data1);

            _packets.Send(new Packet<bool>(IsHost));
            _packets.Send(new Packet<bool>(isNameTaken));

            Name = packetName.Data1;

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
    }
}