﻿using System;
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
        //basic storage api, was created for mocking reasons and because the lambda scope was to complex to understand
        public MessageStorageApi Storage { get; set; }
        
        public RegisterEndPointsAndManageData(ref MessageStorageApi storage )
        {
            this.Storage = storage;
        }
        public RegisterEndPointsAndManageData()
        {
            Storage = new MessageStorageApi();
        }

        //registers endpoints
        public void ChainRegisterEndpoints(ref ServerTcpListener server)
        {
            // registers basic get endpoint where all messages are shown
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

                if(!httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream,"200", response))
                {
                    return -1;
                }                          
                return 200;
            });

            //registers endpoint where specific message is shown
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
                        httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "200", Response);
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond Message not found;
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
            });

            //registers endpoint that allowes posting 
            server.EndPointApi.RegisterEndPoint("POST", "^/messages$", (IRequestContext httpRequest) =>
            {
                Storage.MessageListMutex.WaitOne();
                Storage.MessageList.Add(Storage.MessageCounter, httpRequest.PayLoad.Trim('\n'));
                int tempMessageCounter = Storage.MessageCounter;
                Storage.MessageCounter++;
                Storage.MessageListMutex.ReleaseMutex();
                httpRequest.ReponseHandler.SendDefaultMessage(httpRequest.Stream, "201", tempMessageCounter.ToString());
                return 201;
            });

            //registers endpoint that deletes specific message
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

                        //need to either sort or create a new Dictionary, issue: if u delete a message and post a new one it will no longer be in order
                        Storage.MessageList = new Dictionary<int, string>(Storage.MessageList);

                        Storage.MessageListMutex.ReleaseMutex();
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with bad MessageEndPoint
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
                    return 400;
                }
            });

            //update a specific message if it exists in message list
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
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "200");
                        return 200;
                    }
                    else
                    {
                        Storage.MessageListMutex.ReleaseMutex();
                        //respond with bad MessageEndPoint
                        httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "404");
                        return 404;
                    }
                }
                else
                {
                    //respond with bad Formatting
                    httpRequest.ReponseHandler.SendDefaultStatus(httpRequest.Stream, "400");
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
