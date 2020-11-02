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
        protected static MessageStorageApi Storage;
        protected static EndPointApi<RequestContextInterface,int> EndPointController = new EndPointApi<RequestContextInterface, int>();
        
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

            if (HTTPrequest != null)
            {
                PrintConnectionDetails(HTTPrequest);
            }
            else
            {
                HTTPrequest.ReponseHandler.SendDefaultStatus("400");
                client.Close();
                return;
            }
            int control = EndPointController.InvokeEndPoint(HTTPrequest.HTTPVerb, HTTPrequest.MessageEndPoint, HTTPrequest);
            if (control == -2)
            {
                HTTPrequest.ReponseHandler.SendDefaultStatus("404");
            }
            if (control == -3)
            {
                HTTPrequest.ReponseHandler.SendDefaultStatus("501");
            }
            Console.WriteLine("Thread finished and Client closed");
            client.Close();         
            return;
        }


        static void Main(string[] args)
        {
            Dictionary<int, string> MessageList = new Dictionary<int, string>();
            int MessageCounter = 0;
            Mutex MessageListMutex = new Mutex();

            MessageList = new Dictionary<int, string>();         
            MessageList.Add(0, "Hallo");
            MessageCounter++;
            MessageList.Add(1, "Test");
            MessageCounter++;

            Storage = new MessageStorageApi(ref MessageList, ref MessageCounter, ref MessageListMutex);
            RegisteredEndpoints Controller = new RegisteredEndpoints(ref Storage);
            Controller.ChainRegisterEndpoints(ref EndPointController);

            ServerTcpListener Server = new ServerTcpListener();
            Server.StartServer();
            while (true)
            {               
                    Console.WriteLine("Waiting for a connection... ");
                    TcpClient client = Server.ListenForConnection();
                    Thread ThreadToProcessClient = new Thread(delegate () { ProcessConnection(ref MessageCounter, ref MessageList,Server,ref client); });
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
