using System.IO;

namespace Werewolf.Network.Packets
{
    public class PacketManager
    {
        private readonly Stream _stream;

        public PacketManager(Stream stream)
        {
            _stream = stream;
        }

        public void Send(Packet packet)
        {
            packet.Send(_stream);
        }

        public TPacket Expect<TPacket>() where TPacket : Packet
        {
            return Packet.Receive<TPacket>(_stream);
        }

        public void Close()
        {
            _stream.Close();
        }
    }
}
