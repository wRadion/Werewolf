using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Werewolf.Network
{
    [Serializable]
    public abstract class Packet
    {
        public static T Receive<T>(Stream stream) where T : Packet
        {
            return (T)new BinaryFormatter().Deserialize(stream);
        }

        public void Send(Stream stream)
        {
            new BinaryFormatter().Serialize(stream, this);
        }
    }

    [Serializable]
    public class PacketEvent : Packet
    {
        public dynamic EventArgs { get; }

        public PacketEvent() { }
        public PacketEvent(EventArgs args)
        {
            EventArgs = args;
        }
    }

    [Serializable]
    public class Packet<T1> : Packet
    {
        public T1 Data1 { get; private set; }

        public Packet() { }
        public Packet(T1 data1)
        {
            Data1 = data1;
        }
    }

    [Serializable]
    public class Packet<T1, T2> : Packet
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }

        public Packet() { }
        public Packet(T1 data1, T2 data2)
        {
            Data1 = data1;
            Data2 = data2;
        }
    }

    [Serializable]
    public class Packet<T1, T2, T3> : Packet
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }
        public T3 Data3 { get; private set; }

        public Packet() { }
        public Packet(T1 data1, T2 data2, T3 data3)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
        }
    }
}
