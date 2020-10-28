using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
/* used documentation to implement code:
 * TcpSocket Listener https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=netcore-3.1
 * HttpListener https://stackoverflow.com/questions/9742663/how-do-i-make-http-requests-to-a-tcp-server not allowed but inspiration taken
 */
namespace WebserviceRest
{
    class ServerTcpListener
    {
        public TcpListener Server { get; private set; }
        public IPAddress Adress { get; private set; }
        public int Port = 1235;


        public ServerTcpListener()
        {      
            Adress = IPAddress.Parse("127.0.0.1");
            Port = 1235;
            Server = new TcpListener(Adress, Port);
        }
        public ServerTcpListener(string MyAdress,int Port)
        {
            this.Adress = IPAddress.Parse(MyAdress);
            this.Port = Port;
            Server = new TcpListener(Adress, Port);
        }
        public void StartServer()
        {
            Server.Start();
        }
        public TcpClient ListenForConnection()
        {
            return Server.AcceptTcpClient();
        }
        public RequestContext GetRequestInformationFromConnection(TcpClient Client)
        {
            NetworkStream Stream = Client.GetStream();
            RequestContext RequestInformation = new RequestContext(Client, Stream);
            if (RequestInformation.Parse())
            {
                return RequestInformation;
            }
            else
            {
                return null;
            }
        }
    }
    class RequestContext
    {
        //https://stackoverflow.com/questions/21312870/how-to-access-private-variables-using-get-set
        public string HTTPVerb { get; private set; }
        public string HttpProtokoll { get; private set; }
        public string MessageEndPoint { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public string PayLoad { get; private set; }
        public TcpClient Client { get; private set; } 
        public NetworkStream Stream { get; private set; }
        public RequestContext(TcpClient Client,NetworkStream Stream)
        {
            this.Client = Client;
            this.Stream = Stream;
            Headers = new Dictionary<string, string>();
            HTTPVerb = "";
            HttpProtokoll = "";
            MessageEndPoint = "";
            Headers.Clear();
            PayLoad = "";
        }
        public string[] ResolveEndPointToStringArray()
        {
            return MessageEndPoint.Split('/');
        }
        public void ResetContext()
        {
            //https://stackoverflow.com/questions/1978821/how-to-reset-a-dictionary
            HTTPVerb = "";
            HttpProtokoll = "";
            MessageEndPoint = "";
            Headers.Clear();
            PayLoad = "";
        }
        public string SearchInDictionaryByKey(string FindThisKey)
        {
            //https://stackoverflow.com/questions/5531042/how-to-find-item-in-dictionary-collection
            string Output = "";
            if (Headers.TryGetValue(FindThisKey, out Output))
            {
                return Output;
            }
            else
            {
                return null;
            }
        }
        public bool Parse()
        {
            ResetContext();
            Byte[] bytes = new Byte[2000];
            String data = "";
            string[] SplitByEndline = null;
            string[] SplitBuffer = null;
            bool RecievingHTTPMessage = false;

            int i = 0;
            int counter = 0;
            while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                if (!Stream.DataAvailable)
                {
                    break;
                }
            }
            SplitByEndline = data.Split('\n');
            foreach(string item in SplitByEndline)
            {
                item.Trim('\r');
            }
            SplitBuffer = SplitByEndline[0].Split(' ');
            if (SplitBuffer.Length < 3)
            {
                return false;
            }
            HTTPVerb = SplitBuffer[0];
            MessageEndPoint = SplitBuffer[1];
            HttpProtokoll = SplitBuffer[2];
            SplitByEndline[0] = "\n\n\n\0";
            foreach (string SubString in SplitByEndline)
            {
                if (SubString == "\n\n\n\0")
                {
                    continue;
                }
                if (!RecievingHTTPMessage)
                {
                    if (SubString.Length == 0)
                    {
                        RecievingHTTPMessage = true;
                        continue;
                    }
                    //https://stackoverflow.com/questions/21519548/split-string-based-on-the-first-occurrence-of-the-character
                    SplitBuffer = SubString.Split(new[] { ':' }, 2);
                    if (SplitBuffer.Length == 1)
                    {
                        RecievingHTTPMessage = true;
                        continue;
                    }
                    Headers.Add(SplitBuffer[0], SplitBuffer[1].Trim(' '));
                }
                else
                {
                    PayLoad += SubString + '\n';
                    continue;
                }
            }
            PayLoad = PayLoad.Trim('\n');
            return true;
        }
        public void printdictionary()
        {
            foreach (var item in Headers)
            {
                Console.WriteLine(item.Key + " : " + item.Value);
            }
        }
    }
    class HTTPResponseWrapper
    {
        public Dictionary<string,string> DicionaryHeaders { get;set;}

        public HTTPResponseWrapper()
        {
            DicionaryHeaders = new Dictionary<string, string>();
            ResetContext();
        }
        public void ResetContext()
        {
            DicionaryHeaders.Clear();
        }
        private string ResolveHTTPStatuscode(string StatusCode)
        {
            switch (StatusCode)
            {
                case ("200"): return "OK";
                case ("400"): return "Bad Request";
                case ("404"): return "Not Found";
                case ("201"): return "Created";
                case ("501"): return "Not Implemented";
                default: return "Unknown StatusCode";
            }
        }
        public bool SendResponseByTcp(NetworkStream Stream,string StatusCode)
        {
            if(StatusCode == "")
            {
                return false;
            }
            string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}\r\n";
            foreach (var item in DicionaryHeaders)
            {
                Response+=$"{item.Key}: {item.Value}\r\n";
            }
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(Response);
            Stream.Write(msg, 0, msg.Length);
            return true;
        }
        public bool SendMessageByTcp(NetworkStream Stream, string StatusCode,string Message)
        {
            if (StatusCode == "")
            {
                return false;
            }
            string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}\r\n";
            foreach (var item in DicionaryHeaders)
            {
                Response += $"{item.Key}: {item.Value}\r\n";
            }
            Response += $"Content-Length: {Message.Length}\r\n\r\n{Message}";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(Response);
            Stream.Write(msg, 0, msg.Length);
            return true;
        }
        public bool SendDefaultStatus(NetworkStream Stream, string StatusCode)
        {
            if (StatusCode == "")
            {
                return false;
            }
             string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}\r\nCache-Control: no-cache\nDate: {DateTime.Now}\r\nConnection: Closed";
             byte[] msg = System.Text.Encoding.ASCII.GetBytes(Response);
             Stream.Write(msg, 0, msg.Length);
            return true;
        }
        public bool SendDefaultMessage(NetworkStream Stream, string StatusCode, string Message)
        {
            if (StatusCode == "")
            {
                return false;
            }
            string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}\r\nCache-Control: no-cache\r\nDate: {DateTime.Now}\r\nConnection: Closed\r\nContent-Type: raw\r\nContent-Length: {Message.Length}\r\n\r\n{Message}";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(Response);
            Stream.Write(msg, 0, msg.Length);
            return true;
        }
    }
    class Program
    {
        private static Mutex MessageListMutex = new Mutex();
        private static Mutex MessageCounterMutex = new Mutex();
        static public void endpointMessage(NetworkStream Stream,HTTPResponseWrapper ResponseHandler, RequestContext HTTPrequest,string[] EndPointArray, Dictionary<int, string> MessageList,ref int MessageCounter)
        {
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
                        ResponseHandler.SendDefaultMessage(Stream, "200", Response);
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
                                ResponseHandler.SendDefaultMessage(Stream, "200", Response);
                                break;
                            }
                            else
                            {
                                MessageListMutex.ReleaseMutex();
                                //respond Message not found;
                                ResponseHandler.SendDefaultStatus(Stream, "404");
                                break;
                            }
                        }
                        else
                        {
                            //respond with bad Formatting
                            ResponseHandler.SendDefaultStatus(Stream, "400");
                            break;
                        }
                    }
                    else
                    {
                        //respond with error MEssageEndPoint not exists
                        ResponseHandler.SendDefaultStatus(Stream, "404");
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
                        ResponseHandler.SendDefaultMessage(Stream, "201", tempMessageCounter.ToString());                 
                        break;
                    }
                    else
                    {
                        //respond bad endpoint;
                        ResponseHandler.SendDefaultStatus(Stream, "404");
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
                                ResponseHandler.SendDefaultStatus(Stream, "200");
                                break;
                            }
                            else
                            {
                                MessageListMutex.ReleaseMutex();
                                //respond with bad MessageEndPoint
                                ResponseHandler.SendDefaultStatus(Stream, "404");
                                break;
                            }
                        }
                        else
                        {
                            //respond with bad Formatting
                            ResponseHandler.SendDefaultStatus(Stream, "400");
                            break;
                        }
                    }
                    else
                    {
                        //respond with bad MessageEndPoint
                        ResponseHandler.SendDefaultStatus(Stream, "404");
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
                                ResponseHandler.SendDefaultStatus(Stream, "200");
                                break;
                            }
                            else
                            {
                                MessageListMutex.ReleaseMutex();
                                //respond with bad MessageEndPoint
                                ResponseHandler.SendDefaultStatus(Stream, "404");
                                break;
                            }
                        }
                        else
                        {
                            //respond with bad Formatting
                            ResponseHandler.SendDefaultStatus(Stream, "400");
                        }
                    }
                    else
                    {
                        //respond with bad MessageEndPoint
                        ResponseHandler.SendDefaultStatus(Stream, "404");
                        break;
                    }
                    break;
                default:
                    //respond with HTTPverb not implemented
                    ResponseHandler.SendDefaultStatus(Stream, "501"); ;
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
        static void PrintConnectionDetails(RequestContext HTTPrequest)
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
        static void ProcessConnection(ref Mutex MessageListMutex,ref int MessageCounter,ref Dictionary<int, string> MessageList,ServerTcpListener Server,ref TcpClient client)
        {
            RequestContext HTTPrequest = Server.GetRequestInformationFromConnection(client);
           
            HTTPResponseWrapper ResponseHandler = new HTTPResponseWrapper();
    
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
            endpointMessage(HTTPrequest.Stream, ResponseHandler, HTTPrequest, UrlSplitBySlash, MessageList, ref MessageCounter);
            Console.WriteLine("Thread finished and Client closed");
            client.Close();         
            return;
        }
        static void Main(string[] args)
        {          
            Dictionary<int,string> MessageList = new Dictionary<int, string>();
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
                    Thread ThreadToProcessClient = new Thread(delegate () { ProcessConnection(ref MessageListMutex, ref MessageCounter, ref MessageList,Server,ref client); });
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
