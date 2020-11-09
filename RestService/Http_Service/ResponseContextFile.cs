﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Server;
namespace Restservice.Http_Service
{
    public interface IHTTPResponseWrapper
    {
        public void ResetContext();
        public bool SendResponseByTcp(string StatusCode);
        public bool SendMessageByTcp(string StatusCode, string Message);
        public bool SendDefaultStatus(string StatusCode);
        public bool SendDefaultMessage(string StatusCode, string Message);
        public IMyNetWorkStream Stream { get; }
    }
    public class HTTPResponseWrapper : IHTTPResponseWrapper
    {
        public Dictionary<string, string> DicionaryHeaders { get; set; }
        public IMyNetWorkStream Stream { get; private set; }

        public HTTPResponseWrapper(IMyNetWorkStream Stream)
        {
            DicionaryHeaders = new Dictionary<string, string>();
            this.Stream = Stream;
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
                default: return "Unknown StatusCode";
            }
        }
        public bool SendResponseByTcp(string statusCode)
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
                Stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool SendMessageByTcp(string statusCode, string message)
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
            response += $"Content-Length: {message.Length}\r\n\r\n{message}";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
            try
            {
                Stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            
            return true;
        }
        public bool SendDefaultStatus(string statusCode)
        {
            if (statusCode == "")
            {
                return false;
            }
            string response = $"HTTP/1.1 {statusCode} {ResolveHTTPStatuscode(statusCode)}\r\nCache-Control: no-cache\nDate: {DateTime.Now}\r\nConnection: Closed";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
            try
            {
                Stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool SendDefaultMessage(string statusCode, string message)
        {
            if (statusCode == "")
            {
                return false;
            }
            string response = $"HTTP/1.1 {statusCode} {ResolveHTTPStatuscode(statusCode)}\r\nCache-Control: no-cache\r\nDate: {DateTime.Now}\r\nConnection: Closed\r\nContent-Type: raw\r\nContent-Length: {message.Length}\r\n\r\n{message}";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(response);
            try
            {
                Stream.Write(msg, 0, msg.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
