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
        static void Main(string[] args)
        {
            Dictionary<int, string> messageList = new Dictionary<int, string>();
            int messageCounter = 0;
            Mutex messageListMutex = new Mutex();
        
            messageList.Add(0, "Hallo");
            messageCounter++;
            messageList.Add(1, "Test");
            messageCounter++;

            MessageStorageApi storage = new MessageStorageApi(ref messageList, ref messageCounter, ref messageListMutex);
            RegisterEndPointsAndManageData endPointRegisterController = new RegisterEndPointsAndManageData(ref storage);
            ServerTcpListener server = new ServerTcpListener();
            endPointRegisterController.ChainRegisterEndpoints(ref server);



            server.StartServer();
            server.ListenForConnections();
        }
    }
}
