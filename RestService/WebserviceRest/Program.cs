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
    class RequestContext
    {
        //https://stackoverflow.com/questions/21312870/how-to-access-private-variables-using-get-set
        public string HTTPVerb { get; private set; }
        public string HttpProtokoll { get; private set; }
        public string MessageEndPoint { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public string PayLoad { get; private set; }
        public RequestContext()
        {
            Headers = new Dictionary<string, string>();
            ResetContext();
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
        public bool Parse(NetworkStream Stream)
        {
            ResetContext();
            Byte[] bytes = new Byte[4096];
            String data = null;
            string[] SplitByEndline = null;
            string[] SplitBuffer = null;
            bool RecievingHTTPMessage = false;

            int i = 0;
            int counter = 0;
            while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                if (!RecievingHTTPMessage)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    SplitByEndline = data.Split('\n');
                    if (counter == 0)
                    {
                        SplitBuffer = SplitByEndline[0].Split(' ');
                        if (SplitBuffer.Length < 3)
                        {
                            return false;
                        }
                        HTTPVerb = SplitBuffer[0];
                        MessageEndPoint = SplitBuffer[1];
                        HttpProtokoll = SplitBuffer[2];
                        SplitByEndline[0] = "\n\n\n\0";
                        counter++;
                    }
                    foreach (string SubString in SplitByEndline)
                    {
                        if (SubString == "\n\n\n\0")
                        {
                            continue;
                        }
                        if (!RecievingHTTPMessage)
                        {
                            //https://stackoverflow.com/questions/21519548/split-string-based-on-the-first-occurrence-of-the-character
                            SplitBuffer = SubString.Split(new[] { ':' }, 2);
                            if(SplitBuffer.Length == 1)
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
                }
                else
                {
                    PayLoad += data;
                }
                if (!Stream.DataAvailable)
                {
                    break;
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
            string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}\n";
            foreach (var item in DicionaryHeaders)
            {
                Response+=$"{item.Key}: {item.Value}\n";
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
            string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}\n";
            foreach (var item in DicionaryHeaders)
            {
                Response += $"{item.Key}: {item.Value}\n";
            }
            Response += $"Content-Length: {Message.Length}\n\n{Message}";
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
             string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}Cache-Control: no-cache\nDate: {DateTime.Now}\nConnection: Closed";
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
            string Response = $"HTTP/1.1 {StatusCode} {ResolveHTTPStatuscode(StatusCode)}\nCache-Control: no-cache\nDate: {DateTime.Now}\nConnection: Closed\nContent-Type: raw\nContent-Length: {Message.Length}\n\n{Message}";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(Response);
            Stream.Write(msg, 0, msg.Length);
            return true;
        }
    }
    class Program
    {
        static public void endpointMessage(NetworkStream Stream,HTTPResponseWrapper ResponseHandler, RequestContext HTTPrequest,string[] EndPointArray, Dictionary<int, string> MessageList,ref int MessageCounter)
        {
            switch (HTTPrequest.HTTPVerb)
            {
                case ("GET"):
                    if (EndPointArray.Length == 2 && EndPointArray[1] == "messages")
                    {
                        string Response = "";
                        //https://stackoverflow.com/questions/141088/what-is-the-best-way-to-iterate-over-a-dictionary
                        foreach (KeyValuePair<int, string> MessageKeyValuePair in MessageList)
                        {
                            Response += $"Message { MessageKeyValuePair.Key }: {MessageKeyValuePair.Value}\n";
                        }
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
                            if (MessageList.TryGetValue(MessageIDFromHttpRequest, out Output))
                            {
                                string Response = $"Message {MessageIDFromHttpRequest}: {Output}\n";
                                //respond with OK Message
                                ResponseHandler.SendDefaultMessage(Stream, "200", Response);
                                break;
                            }
                            else
                            {
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
                        MessageList.Add(MessageCounter, HTTPrequest.PayLoad.Trim('\n'));
                        ResponseHandler.SendDefaultMessage(Stream, "201", MessageCounter.ToString());
                        MessageCounter++;
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
                            if (MessageList.ContainsKey(MessageIDFromHttpRequest))
                            {
                                MessageList.Remove(MessageIDFromHttpRequest);
                                ResponseHandler.SendDefaultStatus(Stream, "200");
                                break;
                            }
                            else
                            {
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
                    if (EndPointArray.Length == 3 && EndPointArray[1] == "messages")
                    {
                        int MessageIDFromHttpRequest = ToInt(EndPointArray[2]);
                        if (MessageIDFromHttpRequest != -1)
                        {
                            if (MessageList.ContainsKey(MessageIDFromHttpRequest))
                            {
                                MessageList[MessageIDFromHttpRequest] = HTTPrequest.PayLoad;
                                //respond with ok
                                ResponseHandler.SendDefaultStatus(Stream, "200");
                                break;
                            }
                            else
                            {
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
        static void Main(string[] args)
        {
            TcpListener Server = null;
            IPAddress MyAdress = IPAddress.Parse("127.0.0.1");
            int Port = 1235;
            Server = new TcpListener(MyAdress, Port);
            Server.Start();
            RequestContext HTTPrequest = new RequestContext();
            HTTPResponseWrapper ResponseHandler = new HTTPResponseWrapper();
            Dictionary<int,string> MessageList = new Dictionary<int, string>();
            Byte[] bytes = new Byte[256];
            int MessageCounter = 0;
            MessageList.Add(0, "Hallo");
            MessageCounter++;
            MessageList.Add(1, "Test");
            MessageCounter++;
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = Server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                // Get a stream object for reading and writing
                NetworkStream Stream = client.GetStream();


                if (HTTPrequest.Parse(Stream))
                {
                    Console.WriteLine("HTTP-Verb: "+HTTPrequest.HTTPVerb);
                    Console.WriteLine("HttpProtokoll: "+HTTPrequest.HttpProtokoll);
                    Console.WriteLine("MessageEndPoint: "+HTTPrequest.MessageEndPoint);                  
                    HTTPrequest.printdictionary();
                    if (HTTPrequest.PayLoad.Length > 0)
                    {
                        Console.WriteLine("PayLoad "+HTTPrequest.PayLoad.Length+":\n" + HTTPrequest.PayLoad + "");
                    }                  
                }
                else
                {
                    ResponseHandler.SendDefaultStatus(Stream,"400");
                    client.Close();
                    continue;
                }
                string[] UrlSplitBySlash = HTTPrequest.ResolveEndPointToArray();
                if(UrlSplitBySlash.Length <= 1)
                {
                    //respond with error MessageEndPoint not exists                   
                    ResponseHandler.SendDefaultStatus(Stream, "404");
                    client.Close();
                    continue;
                }
                endpointMessage(Stream, ResponseHandler, HTTPrequest, UrlSplitBySlash, MessageList,ref MessageCounter);
                Console.WriteLine("Client closed");
                client.Close();
            }
        }
    }
}
