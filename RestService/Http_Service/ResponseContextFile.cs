﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Server;
namespace Restservice.Http_Service
{
    public interface HTTPResponseWrapperInterface
    {
        public void ResetContext();
        public bool SendResponseByTcp(string StatusCode);
        public bool SendMessageByTcp(string StatusCode, string Message);
        public bool SendDefaultStatus(string StatusCode);
        public bool SendDefaultMessage(string StatusCode, string Message);
        public FakeNetworkStreamInterface Stream { get; }
    }
    public class HTTPResponseWrapper : HTTPResponseWrapperInterface
    {
        public Dictionary<string, string> DicionaryHeaders { get; set; }
        public FakeNetworkStreamInterface Stream { get; private set; }

        public HTTPResponseWrapper(FakeNetworkStreamInterface Stream)
        {
            DicionaryHeaders = new Dictionary<string, string>();
            this.Stream = Stream;
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
        public bool SendResponseByTcp(string StatusCode)
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
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(Response);
            Stream.Write(msg, 0, msg.Length);
            return true;
        }
        public bool SendMessageByTcp(string StatusCode, string Message)
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
        public bool SendDefaultStatus(string StatusCode)
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
        public bool SendDefaultMessage(string StatusCode, string Message)
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
}
