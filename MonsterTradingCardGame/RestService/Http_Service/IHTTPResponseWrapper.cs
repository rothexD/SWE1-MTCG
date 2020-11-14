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
    public interface IHTTPResponseWrapper
    {
        public void ResetContext();
        public bool SendResponseByTcp(string StatusCode);
        public bool SendMessageByTcp(string StatusCode, string Message);
        public bool SendDefaultStatus(string StatusCode);
        public bool SendDefaultMessage(string StatusCode, string Message);
        public IMyNetWorkStream Stream { get; }
    }
}
