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
    //basic storage api, was created for mocking reasons and because the lambda scope was to complex to understand
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
        public MessageStorageApi(ref Dictionary<int, string> messageList, ref int messageCounter, ref Mutex messageListMutex)
        {
            this.MessageList = messageList;
            this.MessageCounter = messageCounter;
            this.MessageListMutex = messageListMutex;
        }
    }
}
