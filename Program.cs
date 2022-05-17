// See https://aka.ms/new-console-template for more information
using System.Net;
//using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace Program {
	class Packet {
		static IEnumerable<byte[]> FromStream(Stream s) {
			var buf = new byte[1];
			using (var sr = new StreamReader(s)) {
				s.Read(buf, 0, 1);
				var packetSizeExponent = (int) buf[0];
				var packetSize = (int)( 8*Math.Pow(2, packetSizeExponent) );

				s.Read(buf, 0, 1);
				var packetCount        = (ulong) buf[0];

				while(true) {
					buf = new byte[packetSize];

					s.Read(buf, 0, 1);
					var remianingPackets = BitConverter.ToUInt64(buf, 0);
					if(remianingPackets != packetCount--)
						throw new ArgumentOutOfRangeException("packet lost");
					s.Read(buf, 0, packetSize);

					yield return buf;

					if(remianingPackets == 0)
						break;
				}
			}
		}
	}

	class MainClass {
		const int page = 4096;
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
			ProgressBarExample();
		}
	}
}

