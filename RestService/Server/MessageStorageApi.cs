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
    public class MessageStorageApi
    {
        public Dictionary<int, string> MessageList { get; set; }
        public int MessageCounter { get; set; }
        public Mutex MessageListMutex { get; set; }

        public MessageStorageApi()
        {
            MessageList = new Dictionary<int, string>();
            MessageCounter = 0;
            MessageListMutex = new Mutex();
        }
        public MessageStorageApi(ref Dictionary<int, string> MessageList, ref int Messagecounter, ref Mutex MessageListMutex)
        {
            this.MessageList = MessageList;
            this.MessageCounter = Messagecounter;
            this.MessageListMutex = new Mutex();
        }
    }
}
