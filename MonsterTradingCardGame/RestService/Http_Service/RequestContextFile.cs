using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Server;
namespace Restservice.Http_Service
{
    public interface IRequestContext
    {
        public bool GetFromDictionaryByKey(string FindThisKey, out string ReturnVal);
        public bool Parse();
        public void printdictionary();
        public string HTTPVerb { get; }
        public string HttpProtokoll { get;}
        public string MessageEndPoint { get;}
        public Dictionary<string, string> Headers { get;}
        public string PayLoad { get;}
        public IMyTcpClient Client { get;}
        public IMyNetWorkStream Stream { get;}
        
        public IHTTPResponseWrapper ReponseHandler { get; }
    }
    public class RequestContext : IRequestContext
    {
        //https://stackoverflow.com/questions/21312870/how-to-access-private-variables-using-get-set
        public string HTTPVerb { get; private set; }
        public string HttpProtokoll { get; private set; }
        public string MessageEndPoint { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public string PayLoad { get; private set; }
        public IMyTcpClient Client { get; private set; }
        public IMyNetWorkStream Stream { get; private set; }
        public IHTTPResponseWrapper ReponseHandler { get; private set; }
        public RequestContext(IMyTcpClient Client)
        {
            this.Client = Client;
            this.Stream = Client.GetStream();
            ReponseHandler = new HTTPResponseWrapper(Stream);
            Headers = new Dictionary<string, string>();
            HTTPVerb = "";
            HttpProtokoll = "";
            MessageEndPoint = "";
            Headers.Clear();
            PayLoad = "";
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
        public bool GetFromDictionaryByKey(string FindThisKey,out string ReturnVal)
        {
            //https://stackoverflow.com/questions/5531042/how-to-find-item-in-dictionary-collection           
            if (Headers.TryGetValue(FindThisKey, out ReturnVal))
            {
                return true;
            }
            else
            {
                ReturnVal = "";
                return false;
            }
        }
        public bool Parse()
        {
            ResetContext();
            byte[] bytes = new byte[2000];
            string data = "";
            string[] splitByEndline = null;
            string[] splitBuffer = null;
            bool RecievingHTTPMessage = false;

            int i = 0;

            while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                if (!Stream.DataAvailable)
                {
                    break;
                }
            }


            splitByEndline = data.Split('\n');
            for(i=0;i<splitByEndline.Length;i++)
            {
                splitByEndline[i] = splitByEndline[i].Trim('\r');
            }
            splitBuffer = splitByEndline[0].Split(' ');
            if (splitBuffer.Length < 3)
            {
                return false;
            }
            HTTPVerb = splitBuffer[0];
            MessageEndPoint = splitBuffer[1];
            HttpProtokoll = splitBuffer[2];
            splitByEndline[0] = "\n\n\n\0";

            foreach (string subString in splitByEndline)
            {
                if (subString == "\n\n\n\0")
                {
                    continue;
                }
                if (!RecievingHTTPMessage)
                {
                    if (subString.Length == 0)
                    {
                        RecievingHTTPMessage = true;
                        continue;
                    }
                    //https://stackoverflow.com/questions/21519548/split-string-based-on-the-first-occurrence-of-the-character
                    splitBuffer = subString.Split(new[] { ':' }, 2);
                    if (splitBuffer.Length == 1)
                    {
                        RecievingHTTPMessage = true;
                        continue;
                    }
                    Headers.Add(splitBuffer[0], splitBuffer[1].Trim(' '));
                    // host: 127.0.1.2\r\n
                }
                else
                {
                    PayLoad += subString + "\r\n";
                    continue;
                }
            }
            PayLoad = PayLoad.Trim('\n');
            PayLoad = PayLoad.Trim('\r');
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
}
