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
    public class RegisterEndPointsAndManageData
    {
        public MessageStorageApi Storage { get; set; }
        
        public RegisterEndPointsAndManageData(ref MessageStorageApi storage )
        {
            this.Storage = storage;
        }
        public RegisterEndPointsAndManageData()
        {
            Storage = new MessageStorageApi();
        }

        public void ChainRegisterEndpoints(ref ServerTcpListener server)
        {
            server.EndPointApi.RegisterEndPoint("GET", "^/messages$", (IRequestContext httpRequest) =>
            {
                string response = "";
                //https://stackoverflow.com/questions/141088/what-is-the-best-way-to-iterate-over-a-dictionary
                Storage.MessageListMutex.WaitOne();
                foreach (KeyValuePair<int, string> messageKeyValuePair in Storage.MessageList)
                {
                    response += $"Message { messageKeyValuePair.Key }: {messageKeyValuePair.Value}\n";
                }             
                Storage.MessageListMutex.ReleaseMutex();
                //respond with OK Message

                if(!httpRequest.ReponseHandler.SendDefaultMessage("200", response))
                {
                    return -1;
                }                          
                return 200;
            });

            server.EndPointApi.RegisterEndPoint("GET", "^/messages/[0-9]+$", (IRequestContext httpRequest) =>
            {

                string[] endPointArray = httpRequest.MessageEndPoint.Split('/');
                int messageIDFromHttpRequest = ToInt(endPointArray[2]);
                if (messageIDFromHttpRequest != -1)
                {
                    string output = "";
                    Storage.MessageListMutex.WaitOne();
                    if (Storage.MessageList.TryGetValue(messageIDFromHttpRequest, out output))
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        string Response = $"Message {messageIDFromHttpRequest}: {output}\n";
                        //respond with OK Message
                        httpRequest.ReponseHandler.SendDefaultMessage("200", Response);
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond Message not found;
                        httpRequest.ReponseHandler.SendDefaultStatus("404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    httpRequest.ReponseHandler.SendDefaultStatus("400");
                    return 400;
                }
            });

            server.EndPointApi.RegisterEndPoint("POST", "^/messages$", (IRequestContext httpRequest) =>
            {
                Storage.MessageListMutex.WaitOne();
                Storage.MessageList.Add(Storage.MessageCounter, httpRequest.PayLoad.Trim('\n'));
                int tempMessageCounter = Storage.MessageCounter;
                Storage.MessageCounter++;
                Storage.MessageListMutex.ReleaseMutex();
                httpRequest.ReponseHandler.SendDefaultMessage("201", tempMessageCounter.ToString());
                return 201;
            });

            server.EndPointApi.RegisterEndPoint("DELETE", "^/messages/[0-9]+$", (IRequestContext httpRequest) =>
            {
                string[] endPointArray = httpRequest.MessageEndPoint.Split('/');
                int messageIDFromHttpRequest = ToInt(endPointArray[2]);
                if (messageIDFromHttpRequest != -1)
                {
                    Storage.MessageListMutex.WaitOne();
                    if (Storage.MessageList.ContainsKey(messageIDFromHttpRequest))
                    {
                        Storage.MessageList.Remove(messageIDFromHttpRequest);
                        Storage.MessageList = new Dictionary<int, string>(Storage.MessageList);
                        Storage.MessageListMutex.ReleaseMutex();
                        httpRequest.ReponseHandler.SendDefaultStatus("200");
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with bad MessageEndPoint
                        httpRequest.ReponseHandler.SendDefaultStatus("404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    httpRequest.ReponseHandler.SendDefaultStatus("400");
                    return 400;
                }
            });
            server.EndPointApi.RegisterEndPoint("PUT", "^/messages/[0-9]+$", (IRequestContext httpRequest) =>
            {
                string[] endPointArray = httpRequest.MessageEndPoint.Split('/');
                int messageIDFromHttpRequest = ToInt(endPointArray[2]);
                if (messageIDFromHttpRequest != -1)
                {
                    Storage.MessageListMutex.WaitOne();
                    if (Storage.MessageList.ContainsKey(messageIDFromHttpRequest))
                    {
                        Storage.MessageList[messageIDFromHttpRequest] = httpRequest.PayLoad;
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with ok
                        httpRequest.ReponseHandler.SendDefaultStatus("200");
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with bad MessageEndPoint
                        httpRequest.ReponseHandler.SendDefaultStatus("404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    httpRequest.ReponseHandler.SendDefaultStatus("400");
                    return 400;
                }
            });
        }
        static public int ToInt(string number)
        {
            try
            {
                return Int32.Parse(number);
            }
            catch
            {
                return -1;
            }
        }
    }
}
