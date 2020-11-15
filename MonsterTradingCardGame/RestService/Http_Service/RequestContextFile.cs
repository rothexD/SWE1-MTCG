using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Server;
using Restservice.MockHelper;

namespace Restservice.Http_Service
{
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

        public bool Parse()
        {
            ResetContext();
            byte[] bytes = new byte[2048];
            string data = "";
            string[] splitByEndline = null;
            string[] splitBuffer = null;
            bool RecievingHTTPMessage = false;
            int i;

            //parses Socket Message into a string
            while ((i = Stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                if (!Stream.DataAvailable)
                {
                    break;
                }
            }

            //splits the string by \n
            splitByEndline = data.Split('\n');

            //trim of /r
            for(i=0;i<splitByEndline.Length;i++)
            {
                splitByEndline[i] = splitByEndline[i].Trim('\r');
            }
           
            if (splitByEndline.Length == 0)
            {
                return false;
            }
            //split first line by spacebar
            splitBuffer = splitByEndline[0].Split(' ');

            //verify that  3 elements exist in first line
            if (splitBuffer[0].Length < 3)
            {
                return false;
            }

            //assign values.
            HTTPVerb = splitBuffer[0];
            MessageEndPoint = splitBuffer[1];
            HttpProtokoll = splitBuffer[2];
            
            //write an imposible string into splitbyendline so we can skip it in foreach loop.
            splitByEndline[0] = "\n\n\n\0";

            foreach (string subString in splitByEndline)
            {
                if (subString == "\n\n\n\0")
                {
                    continue;
                }

                //check if recieving payload or recieving headers
                if (!RecievingHTTPMessage)
                {
                    //if length = 0 sign for recieving payload
                    if (subString.Length == 0)
                    {
                        RecievingHTTPMessage = true;
                        continue;
                    }
                    //https://stackoverflow.com/questions/21519548/split-string-based-on-the-first-occurrence-of-the-character
                    //split by :
                    splitBuffer = subString.Split(new[] { ':' }, 2);
                    if (splitBuffer.Length == 1)
                    {
                        RecievingHTTPMessage = true;
                        continue;
                    }
                    // example -> host: 127.0.1.2
                    //trim off spacebar
                    Headers.Add(splitBuffer[0], splitBuffer[1].Trim(' '));                  
                }
                else
                {
                    //write payload
                    PayLoad += subString + "\r\n";
                    continue;
                }
            }
            //trim of trailing \r\n
            PayLoad = PayLoad.Trim('\n');
            PayLoad = PayLoad.Trim('\r');
            return true;
        }
        public void printdictionary()
        {
            foreach (var item in Headers)
            {
                Console.WriteLine(item.Key + ": " + item.Value);
            }
        }
    }
}
