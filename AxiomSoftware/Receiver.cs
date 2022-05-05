using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace Axiom.Sockets
{
    public class Receiver
    {
        public event EventHandler<MessageArgs> MessageReceived;
        public Socket socket;

        public Receiver(Socket socket)
        {
            this.socket = socket;
            new Thread(Loop).Start();
        }

        public void Loop()
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                socket.Receive(buffer, SocketFlags.None);
                MessageArgs message = new MessageArgs(Encoding.ASCII.GetString(buffer).Replace("\0", ""));
                MessageReceived.Invoke(this, message);
            }
        }
    }

    public class MessageArgs:EventArgs
    {
        public string Message { get; private set; }

        public MessageArgs(string Message)
        {
            this.Message = Message;
        }
    }
}
