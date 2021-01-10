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
        public bool SendResponseByTcp(IMyNetWorkStream stream,string statusCode);
        public bool SendMessageByTcp(IMyNetWorkStream stream,string statusCode, string message);
        public bool SendDefaultStatus(IMyNetWorkStream Stream,string statusCode);
        public bool SendDefaultMessage(IMyNetWorkStream stream,string statusCode, string message);
    }
}
