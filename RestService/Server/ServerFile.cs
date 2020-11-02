using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Http_Service;
namespace Restservice.Server
{
    class ServerTcpListener
    {
        public TcpListener Server { get; private set; }
        public IPAddress Adress { get; private set; }
        public int Port = 1235;


        public ServerTcpListener()
        {
            Adress = IPAddress.Parse("127.0.0.1");
            Port = 1235;
            Server = new TcpListener(Adress, Port);
        }
        public ServerTcpListener(string MyAdress, int Port)
        {
            this.Adress = IPAddress.Parse(MyAdress);
            this.Port = Port;
            Server = new TcpListener(Adress, Port);
        }
        public void StartServer()
        {
            Server.Start();
        }
        public TcpClient ListenForConnection()
        {
            return Server.AcceptTcpClient();
        }
        public RequestContextInterface GetRequestInformationFromConnection(TcpClient Client)
        {
            FakeNetworkStreamInterface Stream = new MyNetworkStream(Client.GetStream());
            RequestContextInterface RequestInformation = new RequestContext(Client, Stream);
            if (RequestInformation.Parse())
            {
                return RequestInformation;
            }
            else
            {
                return null;
            }
        }
    }
}
