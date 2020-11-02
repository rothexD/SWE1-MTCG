using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Server;
namespace Restservice.Http_Service
{
    public interface RequestContextInterface
    {
        public string[] ResolveEndPointToStringArray();
        public string SearchInDictionaryByKey(string FindThisKey);
        public bool Parse();
        public void printdictionary();
        public string HTTPVerb { get; }
        public string HttpProtokoll { get;}
        public string MessageEndPoint { get;}
        public Dictionary<string, string> Headers { get;}
        public string PayLoad { get;}
        public TcpClient Client { get;}
        public FakeNetworkStreamInterface Stream { get;}
    }
    public class RequestContext : RequestContextInterface
    {
        //https://stackoverflow.com/questions/21312870/how-to-access-private-variables-using-get-set
        public string HTTPVerb { get; private set; }
        public string HttpProtokoll { get; private set; }
        public string MessageEndPoint { get; private set; }
        public Dictionary<string, string> Headers { get; private set; }
        public string PayLoad { get; private set; }
        public TcpClient Client { get; private set; }
        public FakeNetworkStreamInterface Stream { get; private set; }
        public RequestContext(TcpClient Client, FakeNetworkStreamInterface Stream)
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
            for(i=0;i<SplitByEndline.Length;i++)
            {
                SplitByEndline[i] = SplitByEndline[i].Trim('\r');
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
}
