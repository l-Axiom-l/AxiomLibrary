using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Axiom.Sockets
{
    public class Chat
    {
        public List<string> ChatHistory = new List<string>();

        public string ServerIPAddress = "localhost";
        public string ClientIPAddress = "localhost";

        public string DeviceType;

        Socket Sender;

        //IP is the Name of the Computer
        public void ChangeServerIP(string address)
        {
            ServerIPAddress = address;
        }

        public void ChangeClientIP(string address)
        {
            ClientIPAddress = address;
        }

        public void SaveChatHistory(string FolderPath)
        {
            File.WriteAllLines(FolderPath, ChatHistory);
        }

        public void LoadChatHistory(string FilePath)
        {
            ChatHistory.AddRange(File.ReadAllLines(FilePath));
        }

        public void SendMessage(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");
            ChatHistory.Add("Send: " + message);
            Sender.Send(msg);

        }

        public void Update()
        {
            if (ChatHistory == null)
                return;

            Console.Clear();
            foreach (string temp in ChatHistory)
            {
                Console.WriteLine(temp);
            }
        }

        public void StartClient()
        {
            IPHostEntry host = Dns.GetHostEntry(ClientIPAddress);
            Console.WriteLine(Dns.GetHostName());
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11111);
            Sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Sender.Connect(remoteEP);
            byte[] msg = Encoding.ASCII.GetBytes("Test" + "<EOF>");
            Sender.Send(msg);
            //Sender.Shutdown(SocketShutdown.Both);
            //Sender.Close();
        }

        public void StartServer()
        {
            IPHostEntry host = Dns.GetHostEntry(ServerIPAddress);
            IPAddress ipAddress = host.AddressList[0];
            Console.WriteLine(ipAddress.ToString());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11111);
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(localEndPoint);
            socket.Listen(10);
            socket = socket.Accept();
        LOOP:
            string data = null;
            byte[] bytes = null;

            while (true)
            {
                bytes = new byte[1024];
                int bytesRec = socket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }
            ChatHistory.Add("Received: " + data);
            goto LOOP;
        }
    }
}