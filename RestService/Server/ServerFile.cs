using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Http_Service;
namespace Restservice.Server
{
    public class ServerTcpListener 
    {
        public TcpListener Server { get; private set; }
        public IPAddress Adress { get; private set; }
        public int Port = 1235;
        public EndPointApi<IRequestContext,int> EndPointApi { get; protected set; }

        public ServerTcpListener()
        {
            Adress = IPAddress.Parse("127.0.0.1");
            Port = 1235;
            Server = new TcpListener(Adress, Port);
            this.EndPointApi = new EndPointApi<IRequestContext, int>();
        }
        public ServerTcpListener(string myAdress, int port)
        {
            this.Adress = IPAddress.Parse(myAdress);
            this.Port = port;
            Server = new TcpListener(Adress, port);
            this.EndPointApi = new EndPointApi<IRequestContext, int>();
        }
        public void StartServer()
        {
            Server.Start();
        }
        public void ListenForConnections()
        {
            while (true)
            {
                IMyTcpClient client = new MyTcpClient(Server.AcceptTcpClient());
                Thread threadToProcessClient = new Thread(delegate () { ProcessConnection(client); });
                threadToProcessClient.Start();
            }          
        }
        protected IRequestContext GetRequestInformationFromConnection(IMyTcpClient client)
        {
            if (client == null)
            {
                return null; 
            }
            try
            {
                IRequestContext requestInformation = new RequestContext(client);
                if (requestInformation.Parse())
                {
                    return requestInformation;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }                
        }

        protected void PrintConnectionDetails(IRequestContext httpRequest)
        {
            Console.Write(Environment.NewLine);
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine("HTTP-Verb: " + httpRequest.HTTPVerb);
            Console.WriteLine("HttpProtokoll: " + httpRequest.HttpProtokoll);
            Console.WriteLine("MessageEndPoint: " + httpRequest.MessageEndPoint);
            Console.Write(Environment.NewLine);
            httpRequest.printdictionary();
            Console.Write(Environment.NewLine);
            if (httpRequest.PayLoad.Length > 0)
            {
                Console.WriteLine("PayLoad " + httpRequest.PayLoad.Length + ":\n" + httpRequest.PayLoad + "");
            }
            Console.WriteLine("-----------------------------------------------------");
            Console.Write(Environment.NewLine);
        }


        public bool ProcessConnection(IMyTcpClient client)
        {
            if(client == null)
            {                           
                return false;
            }
            IRequestContext httpRequest = GetRequestInformationFromConnection(client);
            try
            {
                
                if (httpRequest != null)
                {
                    PrintConnectionDetails(httpRequest);
                }
                else
                {
                    httpRequest.ReponseHandler.SendDefaultStatus("400");
                    client.Close();
                    return false;
                }       
                int statusCode = EndPointApi.InvokeEndPoint(httpRequest.HTTPVerb, httpRequest.MessageEndPoint, httpRequest);
                if (statusCode == -1)
                {
                    client.Close();
                    return false;
                }
            }
            catch (SocketException)
            {
                client.Close();
                return false;
            }
            catch(ArgumentNullException)
            {
                client.Close();
                return false;
            }
            catch(Exception e) when (e.Message == "NotAValidEndpoint")
            {

                httpRequest.ReponseHandler.SendDefaultStatus("404");
                client.Close();
                return false;
            }
            catch(Exception e) when (e.Message == "NotAValidVerbForEndpoint")
            {
                httpRequest.ReponseHandler.SendDefaultStatus("501");
                client.Close();
                return false;
            }
            
            Console.WriteLine("Thread finished and Client closed");
            client.Close();
            return true;
        }
    }
}
