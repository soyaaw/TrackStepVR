using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TrackStepVR;

/// <summary>Sends OSC float messages over UDP.</summary>
internal sealed class OscSender : IDisposable
{
    private readonly UdpClient client = new();
    private readonly IPEndPoint destination = new(IPAddress.Loopback, 9000);

    public void SendFloat(string address, float value)
    {
        var packet = new List<byte>();
        AppendOscString(packet, address);
        AppendOscString(packet, ",f");
        byte[] argument = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(argument, BitConverter.SingleToInt32Bits(value));
        packet.AddRange(argument);
        byte[] datagram = packet.ToArray();
        client.Send(datagram, datagram.Length, destination);
    }

    public void Dispose() => client.Dispose();

    private static void AppendOscString(List<byte> packet, string value)
    {
        packet.AddRange(Encoding.UTF8.GetBytes(value));
        packet.Add(0);
        while (packet.Count % 4 != 0) packet.Add(0);
    }
}
