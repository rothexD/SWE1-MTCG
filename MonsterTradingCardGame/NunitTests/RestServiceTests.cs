using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Net.Sockets;
using Restservice.Http_Service;
using Restservice.Server;
using Restservice.MockHelper;

namespace NunitTests
{
    class RestServiceTests
    {
        public Mock<IHTTPResponseWrapper> HTTPresponsewrapper;
        public Mock<IRequestContext> RequestContext;
        public Mock<IMyNetWorkStream> Networkstream;
        public Mock<IMyTcpClient> Tcpclient;
        public string Status;
        public int returnStatusCode;
        public string Mesage;
        public Dictionary<int, string> MessageList;
        public Mutex MessageListMutex;
        public int Messagecounter;
        public RegisterEndPointsAndManageData EndpointCreator;
        public MessageStorageApi Storage;
        public ServerTcpListener Server;

        [SetUp]
        public void Construct2()
        {
            MessageList = new Dictionary<int, string>();
            Networkstream = new Mock<IMyNetWorkStream>();
            Tcpclient = new Mock<IMyTcpClient>();
            RequestContext = new Mock<IRequestContext>();
            HTTPresponsewrapper = new Mock<IHTTPResponseWrapper>();
            MessageListMutex = new Mutex();         

            HTTPresponsewrapper.Setup(_ => _.SendDefaultStatus(It.IsAny<string>())).Returns(true).Callback((string y) => { Status = y; });
            HTTPresponsewrapper.Setup(_ => _.SendDefaultMessage(It.IsAny<string>(), It.IsAny<string>())).Returns(true).Callback((string y, string z) => { Status = y; Mesage = z; });
            
            Tcpclient.Setup(_ => _.GetStream()).Returns(Networkstream.Object);
            Tcpclient.Setup(_ => _.Close());

            RequestContext.Setup(_ => _.Stream).Returns(Networkstream.Object);
            RequestContext.Setup(_ => _.ReponseHandler).Returns(HTTPresponsewrapper.Object);            
            Status = "";
            Mesage = "";
            MessageList.Add(0, "test");
            MessageList.Add(1, "abc");

            Messagecounter = MessageList.Count;
            Storage = new MessageStorageApi(ref MessageList, ref Messagecounter, ref MessageListMutex);
            EndpointCreator = new RegisterEndPointsAndManageData(ref Storage);
            Server = new ServerTcpListener();

            EndpointCreator.ChainRegisterEndpoints(ref Server);
        }
        private void SetupMockRequestContext(string verb, string protkoll, string endpoint)
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns(verb);
            RequestContext.Setup(_ => _.HttpProtokoll).Returns(protkoll);
            RequestContext.Setup(_ => _.MessageEndPoint).Returns(endpoint);
        }
        private void SetupMockRequestContext(string verb, string protkoll, string endpoint, string payload)
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns(verb);
            RequestContext.Setup(_ => _.HttpProtokoll).Returns(protkoll);
            RequestContext.Setup(_ => _.MessageEndPoint).Returns(endpoint);
            RequestContext.Setup(_ => _.PayLoad).Returns(payload);
        }
    
        [Test]
        public void testEndPoint_get_200_normalrequest()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "/messages");  
                 
            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, returnStatusCode);
        }
        [Test]
        public void testEndPoint_get_404_BadEndPoint()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "/messages/a");

            Assert.AreEqual("NotAValidEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_get_404_emptyrequest()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "");

            Assert.AreEqual("NotAValidEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_get_404_noleadingslash()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "messages");

            Assert.AreEqual("NotAValidEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_get_200_normalrequesttospecificendpoint()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "/messages/0");
            
            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, returnStatusCode);
        }
        [Test]
        public void testEndPoint_get_404_endpointregextest()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "/messages/0/1");

            Assert.AreEqual("NotAValidEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
            Assert.AreEqual(200, returnStatusCode);
        }
        [Test]
        public void testEndPoint_get_404_messagedoesnotexist()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "/messages/3");

            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(404, returnStatusCode);
        }
        [Test]
        public void testEndPoint_Post_404_posttoinvalidendpoint1()
        {
            SetupMockRequestContext("POST", "HTTP1.0", "/messages/3");

            Assert.AreEqual("NotAValidVerbForEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_Post_404_posttoinvalidendpoint2()
        {
            SetupMockRequestContext("POST", "HTTP1.0", "/messages/a");

            Assert.AreEqual("NotAValidEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_Post_201_postcreated()
        {
            SetupMockRequestContext("POST", "HTTP1.0", "/messages", "PostTest");

            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual("201", Status);
            Assert.AreEqual("PostTest", MessageList[2]);
            Assert.AreEqual(201, returnStatusCode);
        }
        [Test]
        public void testEndPoint_Post_404_invalidendpointalphanumeric()
        {
            SetupMockRequestContext("POST", "HTTP1.0", "/messages/a", "PostTest");

            Assert.AreEqual("NotAValidEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_PUT_200_validputrequest()
        {
            SetupMockRequestContext("PUT", "HTTP1.0", "/messages/0", "PUTTEST");

 
            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, returnStatusCode);
            Assert.AreEqual("PUTTEST", MessageList[0]);
        }
        [Test]
        public void testEndPoint_PUT_404_invalidendpointalphanumeric()
        {
            SetupMockRequestContext("PUT", "HTTP1.0", "/messages/a", "PUTTEST");

            Assert.AreEqual("NotAValidEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_PUT_404_messagedoesnotexist()
        {
            SetupMockRequestContext("PUT", "HTTP1.0", "/messages/5", "PUTTEST");

            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(404, returnStatusCode);
        }
        [Test]
        public void testEndPoint_Delete_404_invalidendpoint()
        {
            SetupMockRequestContext("DELETE", "HTTP1.0", "/messages");

            Assert.AreEqual("NotAValidVerbForEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testEndPoint_Delete_404_doesnotexist()
        {
            SetupMockRequestContext("DELETE", "HTTP1.0", "/messages/5");

            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(404, returnStatusCode);
        }
        [Test]
        public void testEndPoint_Delete_404_successfuldelete()
        {
            SetupMockRequestContext("DELETE", "HTTP1.0", "/messages/0");

            returnStatusCode = Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, returnStatusCode);
        }
        [Test]
        public void testEndPoint_unknownhttpverb_501()
        {
            SetupMockRequestContext("Hans", "HTTP1.0", "/messages/0");

            Assert.AreEqual("NotAValidVerbForEndpoint", Assert.Throws<Exception>(() => Server.EndPointApi.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object)).Message);
        }
        [Test]
        public void testParse()
        {
            Mock<IMyNetWorkStream> myFakeStream = new Mock<IMyNetWorkStream>();
            byte[] tempArray = new byte[2000];
            RequestContext ContextTest;
            string testString;


            testString = "GET /messages HTTP/1.1\r\nHost:127.0.0.1\r\nTest:Test\r\n\r\nPayloadTest";
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(testString), tempArray, testString.Length);   
            
            myFakeStream.Setup(_ => _.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns((byte[] x, int y, int z) => { Array.Copy(tempArray,0,x,0,testString.Length); return testString.Length; });            
            myFakeStream.Setup(_ => _.DataAvailable).Returns(false);
            Tcpclient.Setup(_ => _.GetStream()).Returns(myFakeStream.Object);

            ContextTest = new RequestContext(Tcpclient.Object);
            ContextTest.Parse();
            ContextTest.Headers.TryGetValue("Host", out string Key1);
            ContextTest.Headers.TryGetValue("Test", out string Key2);



            Assert.AreEqual("GET", ContextTest.HTTPVerb);
            Assert.AreEqual("/messages", ContextTest.MessageEndPoint);
            Assert.AreEqual("HTTP/1.1", ContextTest.HttpProtokoll);
            Assert.AreEqual("PayloadTest", ContextTest.PayLoad);
            Assert.AreEqual("127.0.0.1", Key1);
            Assert.AreEqual("Test", Key2);          
        }
        [Test]
        public void TestProcessConnection_nullref()
        {
            bool returnVal;

            returnVal = Server.ProcessConnection(null);

            Assert.AreEqual(false, returnVal);           
        }
        [Test]
        public void TestProcessConnection_validrequest()
        {        
            Mock<IMyNetWorkStream> myFakeStream = new Mock<IMyNetWorkStream>();
            byte[] tempArray = new byte[2000];
            bool returnVal; 
            string testString;


            testString = "GET /messages HTTP/1.1\r\nHost:127.0.0.1\r\nTest:Test\r\n\r\nPayloadTest";
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(testString), tempArray, testString.Length);      
            myFakeStream.Setup(_ => _.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns((byte[] x, int y, int z) => { Array.Copy(tempArray, 0, x, 0, testString.Length); return testString.Length; });
            myFakeStream.Setup(_ => _.DataAvailable).Returns(false);         
            Tcpclient.Setup(_ => _.GetStream()).Returns(myFakeStream.Object);
            
            returnVal = Server.ProcessConnection(Tcpclient.Object);


            Assert.AreEqual(true, returnVal);
        }
        [Test]
        public void TestProcessConnection_unknownverb()
        {
            Mock<IMyNetWorkStream> myFakeStream = new Mock<IMyNetWorkStream>();
            byte[] tempArray = new byte[2000];
            string teststring;
            bool returnval;


            teststring = "HANS /messages HTTP/1.1\r\nHost:127.0.0.1\r\nTest:Test\r\n\r\nPayloadTest";
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(teststring), tempArray, teststring.Length);
            myFakeStream.Setup(_ => _.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns((byte[] x, int y, int z) => { Array.Copy(tempArray, 0, x, 0, teststring.Length); return teststring.Length; });
            myFakeStream.Setup(_ => _.DataAvailable).Returns(false);
            Tcpclient.Setup(_ => _.GetStream()).Returns(myFakeStream.Object);

            returnval = Server.ProcessConnection(Tcpclient.Object);

            Assert.AreEqual(false, returnval);
        }
        [Test]
        public void TestProcessConnection_unknownendpoint()
        {
            Mock<IMyNetWorkStream> myFakeStream = new Mock<IMyNetWorkStream>();
            byte[] tempArray = new byte[2048];
            string testString;
            bool returnVal;


            testString = "GET /messag HTTP/1.1\r\nHost:127.0.0.1\r\nTest:Test\r\n\r\nPayloadTest";
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(testString), tempArray, testString.Length);
            myFakeStream.Setup(_ => _.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns((byte[] x, int y, int z) => { Array.Copy(tempArray, 0, x, 0, testString.Length); return testString.Length; });
            myFakeStream.Setup(_ => _.DataAvailable).Returns(false);
            Tcpclient.Setup(_ => _.GetStream()).Returns(myFakeStream.Object);

            returnVal = Server.ProcessConnection(Tcpclient.Object);

            Assert.AreEqual(false, returnVal);
        }
        [Test]
        public void TestProcessConnection_socketexception()
        {
            Mock<IMyNetWorkStream> MyFakeStream = new Mock<IMyNetWorkStream>();
            byte[] tempArray = new byte[2048];
            string testString;
            bool returnVal;

            testString = "GET /message HTTP/1.1\r\nHost:127.0.0.1\r\nTest:Test\r\n\r\nPayloadTest";
            
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(testString), tempArray, testString.Length);
            MyFakeStream.Setup(_ => _.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns((byte[] x, int y, int z) => { Array.Copy(tempArray, 0, x, 0, testString.Length); return testString.Length; });
            MyFakeStream.Setup(_ => _.DataAvailable).Returns(false);
            MyFakeStream.Setup(_ => _.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new SocketException(1));
            Tcpclient.Setup(_ => _.GetStream()).Returns(MyFakeStream.Object);

            returnVal = Server.ProcessConnection(Tcpclient.Object);

            Assert.AreEqual(false, returnVal);
        }
        [Test]
        public void TestProcessConnection_argumentnullexcpetion()
        {
            Mock<IMyNetWorkStream> myFakeStream = new Mock<IMyNetWorkStream>();
            byte[] tempArray = new byte[2048];
            string testString;
            bool returnVal;

            testString = "GET /message HTTP/1.1\r\nHost:127.0.0.1\r\nTest:Test\r\n\r\nPayloadTest";
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(testString), tempArray, testString.Length);

            myFakeStream.Setup(_ => _.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns((byte[] x, int y, int z) => { Array.Copy(tempArray, 0, x, 0, testString.Length); return testString.Length; });
            myFakeStream.Setup(_ => _.DataAvailable).Returns(false);
            myFakeStream.Setup(_ => _.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new ArgumentNullException());
            Tcpclient.Setup(_ => _.GetStream()).Returns(myFakeStream.Object);

            returnVal = Server.ProcessConnection(Tcpclient.Object);

            Assert.AreEqual(false, returnVal);
        }
    }
}
