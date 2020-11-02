using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Http_Service;
using Restservice.Server;
using Restservice;

namespace Restservice.Server
{
    

    public class RegisteredEndpoints
    {
        public MessageStorageApi Storage { get; set; }
        
        public RegisteredEndpoints(ref MessageStorageApi Storage )
        {
            this.Storage = Storage;
    }
        public RegisteredEndpoints()
        {

        }
        public void ChainRegisterEndpoints(ref EndPointApi<RequestContextInterface, int> EndPointController)
        {
            EndPointController.RegisterEndPoint("GET", "/messages", (HTTPrequest) =>
            {
                string Response = "";
                //https://stackoverflow.com/questions/141088/what-is-the-best-way-to-iterate-over-a-dictionary
                Storage.MessageListMutex.WaitOne();
                foreach (KeyValuePair<int, string> MessageKeyValuePair in Storage.MessageList)
                {
                    Response += $"Message { MessageKeyValuePair.Key }: {MessageKeyValuePair.Value}\n";
                }
                Storage.MessageListMutex.ReleaseMutex();
                //respond with OK Message
                HTTPrequest.ReponseHandler.SendDefaultMessage("200", Response);
                return 200;
            });

            EndPointController.RegisterEndPoint("GET", "/messages/[0-9]+", (HTTPrequest) =>
            {

                string[] EndPointArray = HTTPrequest.ResolveEndPointToStringArray();
                int MessageIDFromHttpRequest = ToInt(EndPointArray[2]);
                if (MessageIDFromHttpRequest != -1)
                {
                    string Output = "";
                    Storage.MessageListMutex.WaitOne();
                    if (Storage.MessageList.TryGetValue(MessageIDFromHttpRequest, out Output))
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        string Response = $"Message {MessageIDFromHttpRequest}: {Output}\n";
                        //respond with OK Message
                        HTTPrequest.ReponseHandler.SendDefaultMessage("200", Response);
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond Message not found;
                        HTTPrequest.ReponseHandler.SendDefaultStatus("404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    HTTPrequest.ReponseHandler.SendDefaultStatus("400");
                    return 400;
                }
            });

            EndPointController.RegisterEndPoint("POST", "/messages", (HTTPrequest) =>
            {
                Storage.MessageListMutex.WaitOne();
                Storage.MessageList.Add(Storage.MessageCounter, HTTPrequest.PayLoad.Trim('\n'));
                int tempMessageCounter = Storage.MessageCounter;
                Storage.MessageCounter++;
                Storage.MessageListMutex.ReleaseMutex();
                HTTPrequest.ReponseHandler.SendDefaultMessage("201", tempMessageCounter.ToString());
                return 201;
            });

            EndPointController.RegisterEndPoint("DELETE", "/messages/[0-9]+", (HTTPrequest) =>
            {
                string[] EndPointArray = HTTPrequest.ResolveEndPointToStringArray();
                int MessageIDFromHttpRequest = ToInt(EndPointArray[2]);
                if (MessageIDFromHttpRequest != -1)
                {
                    Storage.MessageListMutex.WaitOne();
                    if (Storage.MessageList.ContainsKey(MessageIDFromHttpRequest))
                    {
                        Storage.MessageList.Remove(MessageIDFromHttpRequest);
                        Storage.MessageListMutex.ReleaseMutex();
                        HTTPrequest.ReponseHandler.SendDefaultStatus("200");
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with bad MessageEndPoint
                        HTTPrequest.ReponseHandler.SendDefaultStatus("404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    HTTPrequest.ReponseHandler.SendDefaultStatus("400");
                    return 400;
                }
            });
            EndPointController.RegisterEndPoint("PUT", "/messages/[0-9]+", (HTTPrequest) =>
            {
                string[] EndPointArray = HTTPrequest.ResolveEndPointToStringArray();
                int MessageIDFromHttpRequest = ToInt(EndPointArray[2]);
                if (MessageIDFromHttpRequest != -1)
                {
                    Storage.MessageListMutex.WaitOne();
                    if (Storage.MessageList.ContainsKey(MessageIDFromHttpRequest))
                    {
                        Storage.MessageList[MessageIDFromHttpRequest] = HTTPrequest.PayLoad;
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with ok
                        HTTPrequest.ReponseHandler.SendDefaultStatus("200");
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with bad MessageEndPoint
                        HTTPrequest.ReponseHandler.SendDefaultStatus("404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    HTTPrequest.ReponseHandler.SendDefaultStatus("400");
                    return 400;
                }
            });
        }
        static public int ToInt(string Number)
        {
            try
            {
                return Int32.Parse(Number);
            }
            catch
            {
                return -1;
            }
        }
    }
}
