using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Http_Service;
using Restservice.Server;
/* used documentation to implement code:
 * TcpSocket Listener https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=netcore-3.1
 * HttpListener https://stackoverflow.com/questions/9742663/how-do-i-make-http-requests-to-a-tcp-server not allowed but inspiration taken
 */
namespace WebserviceRest
{
    public class Program
    {
        public static Mutex MessageListMutex = new Mutex();
        Dictionary<int, string> MessageList;
        static public void endpointMessage(HTTPResponseWrapperInterface ResponseHandler, RequestContextInterface HTTPrequest,ref Dictionary<int, string> MessageList,ref int MessageCounter)
        {
            string[] EndPointArray = HTTPrequest.ResolveEndPointToStringArray();
            switch (HTTPrequest.HTTPVerb)
            {
                case ("GET"):
                    if (EndPointArray.Length == 2 && EndPointArray[1] == "messages")
                    {
                        string Response = "";
                        //https://stackoverflow.com/questions/141088/what-is-the-best-way-to-iterate-over-a-dictionary
                        MessageListMutex.WaitOne();
                        foreach (KeyValuePair<int, string> MessageKeyValuePair in MessageList)
                        {
                            Response += $"Message { MessageKeyValuePair.Key }: {MessageKeyValuePair.Value}\n";
                        }
                        MessageListMutex.ReleaseMutex();
                        //respond with OK Message
                        ResponseHandler.SendDefaultMessage(HTTPrequest.Stream, "200", Response);
                        break;
                    }
                    else if (EndPointArray.Length == 3 && EndPointArray[1] == "messages")
                    {
                        int MessageIDFromHttpRequest = ToInt(EndPointArray[2]);
                        if (MessageIDFromHttpRequest != -1)
                        {
                            string Output = "";
                            MessageListMutex.WaitOne();
                            if (MessageList.TryGetValue(MessageIDFromHttpRequest, out Output))
                            {
                                MessageListMutex.ReleaseMutex();
                                string Response = $"Message {MessageIDFromHttpRequest}: {Output}\n";
                                //respond with OK Message
                                ResponseHandler.SendDefaultMessage(HTTPrequest.Stream, "200", Response);
                                break;
                            }
                            else
                            {
                                MessageListMutex.ReleaseMutex();
                                //respond Message not found;
                                ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                                break;
                            }
                        }
                        else
                        {
                            //respond with bad Formatting
                            ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "400");
                            break;
                        }
                    }
                    else
                    {
                        //respond with error MEssageEndPoint not exists
                        ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                        break;
                    }
                case ("POST"):
                    if (EndPointArray.Length == 2 && EndPointArray[1] == "messages")
                    {
                        MessageListMutex.WaitOne();
                        MessageList.Add(MessageCounter, HTTPrequest.PayLoad.Trim('\n'));
                        int tempMessageCounter = MessageCounter;
                        MessageCounter++;
                        MessageListMutex.ReleaseMutex();
                        ResponseHandler.SendDefaultMessage(HTTPrequest.Stream, "201", tempMessageCounter.ToString());                 
                        break;
                    }
                    else
                    {
                        //respond bad endpoint;
                        ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                        break;
                    }
                case ("DELETE"):
                    if (EndPointArray.Length == 3 && EndPointArray[1] == "messages")
                    {
                        int MessageIDFromHttpRequest = ToInt(EndPointArray[2]);
                        if (MessageIDFromHttpRequest != -1)
                        {
                            MessageListMutex.WaitOne();
                            if (MessageList.ContainsKey(MessageIDFromHttpRequest))
                            {
                                MessageList.Remove(MessageIDFromHttpRequest);
                                MessageListMutex.ReleaseMutex();                              
                                ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "200");
                                break;
                            }
                            else
                            {
                                MessageListMutex.ReleaseMutex();
                                //respond with bad MessageEndPoint
                                ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                                break;
                            }
                        }
                        else
                        {
                            //respond with bad Formatting
                            ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "400");
                            break;
                        }
                    }
                    else
                    {
                        //respond with bad MessageEndPoint
                        ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                        break;
                    }
                case ("PUT"):
                    Console.WriteLine("ENTERED PUT");
                    if (EndPointArray.Length == 3 && EndPointArray[1] == "messages")
                    {
                        int MessageIDFromHttpRequest = ToInt(EndPointArray[2]);
                        if (MessageIDFromHttpRequest != -1)
                        {
                            MessageListMutex.WaitOne();
                            if (MessageList.ContainsKey(MessageIDFromHttpRequest))
                            {
                                MessageList[MessageIDFromHttpRequest] = HTTPrequest.PayLoad;
                                MessageListMutex.ReleaseMutex();
                                //respond with ok
                                ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "200");
                                break;
                            }
                            else
                            {
                                MessageListMutex.ReleaseMutex();
                                //respond with bad MessageEndPoint
                                ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                                break;
                            }
                        }
                        else
                        {
                            //respond with bad Formatting
                            ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "400");
                        }
                    }
                    else
                    {
                        //respond with bad MessageEndPoint
                        ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                        break;
                    }
                    break;
                default:
                    //respond with HTTPverb not implemented
                    ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "501"); ;
                    break;
            }
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
        static void PrintConnectionDetails(RequestContextInterface HTTPrequest)
        {
            Console.Write(Environment.NewLine);
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("HTTP-Verb: " + HTTPrequest.HTTPVerb);
            Console.WriteLine("HttpProtokoll: " + HTTPrequest.HttpProtokoll);
            Console.WriteLine("MessageEndPoint: " + HTTPrequest.MessageEndPoint);
            Console.Write(Environment.NewLine);
            HTTPrequest.printdictionary();
            Console.Write(Environment.NewLine);
            if (HTTPrequest.PayLoad.Length > 0)
            {
                Console.WriteLine("PayLoad " + HTTPrequest.PayLoad.Length + ":\n" + HTTPrequest.PayLoad + "");
            }
            Console.WriteLine("-----------------------------------------------------");
            Console.Write(Environment.NewLine);
        }
        static void ProcessConnection(ref int MessageCounter,ref Dictionary<int, string> MessageList,ServerTcpListener Server,ref TcpClient client)
        {
            RequestContextInterface HTTPrequest = Server.GetRequestInformationFromConnection(client);

            HTTPResponseWrapperInterface ResponseHandler = new HTTPResponseWrapper();
    
            if (HTTPrequest != null)
            {
                PrintConnectionDetails(HTTPrequest);
            }
            else
            {
                ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "400");
                client.Close();
                return;
            }
            string[] UrlSplitBySlash = HTTPrequest.ResolveEndPointToStringArray();
            if (UrlSplitBySlash.Length <= 1)
            {
                //respond with error MessageEndPoint not exists                   
                ResponseHandler.SendDefaultStatus(HTTPrequest.Stream, "404");
                client.Close();
                return;
            }
            endpointMessage(ResponseHandler, HTTPrequest,ref MessageList, ref MessageCounter);
            Console.WriteLine("Thread finished and Client closed");
            client.Close();         
            return;
        }
        static void Main(string[] args)
        {          
            MessageList = new Dictionary<int, string>();
            Byte[] bytes = new Byte[256];
            int MessageCounter = 0;
            MessageList.Add(0, "Hallo");
            MessageCounter++;
            MessageList.Add(1, "Test");
            MessageCounter++;
            Dictionary<TcpClient,Thread> ThreadList = new Dictionary<TcpClient, Thread>();
            ServerTcpListener Server = new ServerTcpListener();
            Server.StartServer();
            while (true)
            {               
                    Console.WriteLine("Waiting for a connection... ");
                    TcpClient client = Server.ListenForConnection();
                    Thread ThreadToProcessClient = new Thread(delegate () { ProcessConnection(ref MessageCounter, ref MessageList,Server,ref client); });
                    ThreadToProcessClient.Name = "Hans";
                    ThreadList.Add(client, ThreadToProcessClient);
                    ThreadToProcessClient.Start();

                    /*Console.WriteLine("Clean Up");

                    List<TcpClient>TempList = new List<TcpClient>();
                    foreach (var item in ThreadList)
                    {
                        if (item.Value.Join(20))
                        {
                            TempList.Add(item.Key);
                            Console.WriteLine("One thread joined");
                        }
                    }
                    foreach (var item in TempList)
                    {
                    ThreadList.Remove(item);
                    Console.WriteLine("Removed thread from List");
                    }*/
            }
        }
    }
}
