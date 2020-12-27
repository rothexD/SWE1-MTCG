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
    
    public class HTTPResponseWrapper : IHTTPResponseWrapper
    {
        public Dictionary<string, string> DicionaryHeaders { get; set; }

        public HTTPResponseWrapper()
        {
            DicionaryHeaders = new Dictionary<string, string>();
            ResetContext();
        }
        public void ResetContext()
        {
            DicionaryHeaders.Clear();
        }
        private string ResolveHTTPStatuscode(string statusCode)
        {
            switch (statusCode)
            {
                case ("200"): return "OK";
                case ("400"): return "Bad Request";
                case ("404"): return "Not Found";
                case ("201"): return "Created";
                case ("501"): return "Not Implemented";
                case ("202"): return "Accepted";
                case ("403"): return "Forbidden";
                case ("500"): return "Internal Server Error";

                default: return "Unknown StatusCode";
            }
        }
        public bool SendResponseByTcp(IMyNetWorkStream stream,string statusCode)
        {
            if (statusCode == "")
            {
                return false;
            }
            string response = $"HTTP/1.1 {statusCode} {ResolveHTTPStatuscode(statusCode)}\r\n";

            foreach (var item in DicionaryHeaders)
            {
                response += $"{item.Key}: {item.Value}\r\n";
            }
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool SendMessageByTcp(IMyNetWorkStream stream, string statusCode, string message)
        {
            if (statusCode == "")
            {
                return false;
            }
            string response = $"HTTP/1.1 {statusCode} {ResolveHTTPStatuscode(statusCode)}\r\n";
            foreach (var item in DicionaryHeaders)
            {
                response += $"{item.Key}: {item.Value}\r\n";
            }
            response += "\r\n";
            response += $"Content-Length: {message.Length}\r\n\r\n{message}";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            
            return true;
        }
        public bool SendDefaultStatus(IMyNetWorkStream stream,string statusCode)
        {
            if (statusCode == "")
            {
                return false;
            }
            string response = $"HTTP/1.1 {statusCode} {ResolveHTTPStatuscode(statusCode)}\r\nCache-Control: no-cache\nDate: {DateTime.Now}\r\nConnection: Closed\r\n\r\n";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool SendDefaultMessage(IMyNetWorkStream stream,string statusCode, string message)
        {
            if (statusCode == "")
            {
                return false;
            }
            string response = $"HTTP/1.1 {statusCode} {ResolveHTTPStatuscode(statusCode)}\r\nCache-Control: no-cache\r\nDate: {DateTime.Now}\r\nConnection: Closed\r\nContent-Type: application/json\r\nContent-Length: {message.Length}\r\n\r\n{message}";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
            try
            {
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
