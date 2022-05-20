// See https://aka.ms/new-console-template for more information
using System.Net;
//using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace Program {
	class Packet {
		public static byte[] FromStream(Stream s) {
			var packetSizeExponent = s.ReadByte();
			var packetSize = (int) (8*Math.Pow(2, packetSizeExponent));
			var buffer = new byte[packetSize];
			s.Read(buffer, 0, (int) packetSize);
			return buffer;
		}

		public static void ToStream(Stream s, byte[] data) {
			var dataSize = data.Length;
			var packetSizeExponent = (byte) Math.Ceiling(Math.Log2(dataSize/8));
			s.WriteByte((byte)packetSizeExponent);
			var packetSize = (int) (8*Math.Pow(2, packetSizeExponent));
			var paddedData = new byte[packetSize];
			Buffer.BlockCopy(data, 0, paddedData, 0, dataSize);
			s.Write(paddedData, 0, packetSize);
		}

		static byte[] ToBytes(string x) {
			return Encoding.ASCII.GetBytes(x);
		}

		static string FromBytes(byte[] x) {
			return Encoding.ASCII.GetString(x);
		}

		public static string StringFromStream(Stream s) {
			var packetSizeExponent = s.ReadByte();
			var packetSize = (int) (8*Math.Pow(2, packetSizeExponent));
			var buffer = new byte[packetSize];
			s.Read(buffer, 0, (int) packetSize);
			return FromBytes(buffer);
		}

		public static void StringToStream(Stream s, string rawData) {
			var data = ToBytes(rawData);
			var dataSize = data.Length;
			var packetSizeExponent = (byte) Math.Ceiling(Math.Log2(dataSize/8));
			s.WriteByte((byte)packetSizeExponent);
			var packetSize = (int) (8*Math.Pow(2, packetSizeExponent));
			var paddedData = new byte[packetSize];
			Buffer.BlockCopy(data, 0, paddedData, 0, dataSize);
			s.Write(paddedData, 0, packetSize);
		}
	}

	class MainClass {
		const int port = 1337;

		static byte[] ToBytes(string x) {
			return Encoding.ASCII.GetBytes(x);
		}

		static string FromBytes(byte[] x) {
			return Encoding.ASCII.GetString(x);
		}

		static void SocketExample() {
			var msg = "hello!";
			var len = Encoding.ASCII.GetByteCount(msg);
			var sendData = Encoding.ASCII.GetBytes(msg);
			var page = 4096;
			byte[] recvData = new byte[page];

			var server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
			server.Start();
			var client = new TcpClient("127.0.0.1", port);

			var serverStream = server.AcceptTcpClient().GetStream();
			var clientStream = client.GetStream();

			Console.WriteLine($"[sent]:  {Encoding.ASCII.GetString(sendData)}");
			clientStream.Write(sendData, 0, len);
			serverStream.Read(recvData, 0, len);
			Console.WriteLine($"[recvd]: {Encoding.ASCII.GetString(recvData)}");
		}

		static void SocketPacketExample() {
			var msg = "a verry long message, I swear!";

			var server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
			server.Start();
			var client = new TcpClient("127.0.0.1", port);

			var serverStream = server.AcceptTcpClient().GetStream();
			var clientStream = client.GetStream();

			Packet.ToStream(clientStream, ToBytes(msg));
			Console.WriteLine($"[sent]:  {msg}");
			var recvData = FromBytes(Packet.FromStream(serverStream));
			Console.WriteLine($"[recvd]: {recvData}");
		}

		static void ProgressBarExample(int len=20, int dt=500) {
			var padding      = (int len) => new string(' ', len);
			var bar          = (int len, int full) => new string('#', full) + padding(len-full);
			var decoratedBar = (int len, int full) => '[' + bar(len-2, full) + ']';
			var blankScreen  = () => new string(' ', Console.WindowWidth * Console.WindowHeight);

			Console.CursorTop = 0;
			Console.CursorLeft = 0;
			Console.Write(blankScreen());
			Console.CursorTop = 0;
			Console.CursorLeft = 0;

			int barSize = len+2;
			Console.Write(decoratedBar(barSize, 0));
			for(int i=0; i<len; i++) {
				string output = decoratedBar(barSize, i+1);
				Thread.Sleep(dt);
				Console.CursorTop = 0;
				Console.CursorLeft = 0;
				Console.Write(output);
			}
		}

		static void Main(string[] args) {
			SocketPacketExample();
		}
	}
}

