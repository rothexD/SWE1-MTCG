using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Http_Service;
namespace Restservice.MockHelper
{

    public class MyNetWorkStream : IMyNetWorkStream
    {
        private NetworkStream _stream { get; set; }

        public MyNetWorkStream(NetworkStream ns)
        {
            if (ns == null) throw new ArgumentNullException("ns");
            this._stream = ns;
        }

        public bool DataAvailable
        {
            get
            {
                return this._stream.DataAvailable;
            }
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            return this._stream.Read(buffer, offset, size);
        }
        public void Write(byte[] buffer, int offset, int size)
        {
            this._stream.Write(buffer, offset, size);
        }
    }

}