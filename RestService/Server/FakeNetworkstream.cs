using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Http_Service;
namespace Restservice.Server
{
    public interface FakeNetworkStreamInterface
    {

        int Read(byte[] buffer, int offset, int size);
        bool DataAvailable { get; }
        void Write(byte[] buffer, int offset, int size);

    }
    public class MyNetworkStream : FakeNetworkStreamInterface
    {
        private NetworkStream stream;

        public MyNetworkStream(NetworkStream ns)
        {
            if (ns == null) throw new ArgumentNullException("ns");
            this.stream = ns;
        }

        public bool DataAvailable
        {
            get
            {
                return this.stream.DataAvailable;
            }
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            return this.stream.Read(buffer, offset, size);
        }
        public void Write(byte[] buffer, int offset, int size)
        {
            this.stream.Write(buffer, offset, size);
        }
    }

}