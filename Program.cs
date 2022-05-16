// See https://aka.ms/new-console-template for more information
using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace Program {
	class MainClass {
		const int page = 4096;
		const int port = 1337;

		static void Main(string[] args) {

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
	}
}


