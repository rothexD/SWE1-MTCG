using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Restservice.Http_Service;
using Restservice.Server;
using Restservice;
using System.Threading;
using System.Net;

namespace Restservice.Server
{
    public interface IMyTcpClient
    {
        public IMyNetWorkStream GetStream();
        public void Close();
    }
    class MyTcpClient : IMyTcpClient
    {
        public TcpClient Client { get ;private set;}
        public MyTcpClient(TcpClient client)
        {
            this.Client = client;
        }
        public IMyNetWorkStream GetStream()
        {
            return new MyNetWorkStream(Client.GetStream());
        }
        public void Close()
        {
            Client.Close();
        }
    }
}
