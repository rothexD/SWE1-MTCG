using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Server;
using Restservice.MockHelper;

namespace Restservice.Http_Service
{
    public interface IRequestContext
    {
        public bool Parse();
        public void printdictionary();
        public string HTTPVerb { get; }
        public string HttpProtokoll { get; }
        public string MessageEndPoint { get; }
        public Dictionary<string, string> Headers { get; }
        public string PayLoad { get; }
        public IMyTcpClient Client { get; }
        public IMyNetWorkStream Stream { get; }
        public IHTTPResponseWrapper ReponseHandler { get; }
    }
}
