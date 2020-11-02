using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using WebserviceRest;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Restservice.Http_Service;
using Restservice.Server;


namespace NunitTests
{
    class RestServiceTests
    {
        public Mock<HTTPResponseWrapperInterface> HTTPresponsewrapper;
        public Mock<RequestContextInterface> RequestContext;
        public Mock<FakeNetworkStreamInterface> Networkstream;
        public Mock<TcpClient> Tcpclient;
        public string status;
        public int revokestatuscode;
        public string message;
        public EndPointApi<RequestContextInterface, int> EndPointController;
        public Dictionary<int, string> MessageList;
        public Mutex MessageListMutex;
        public int Messagecounter;
        public RegisteredEndpoints EndpointCreator;
        public MessageStorageApi Storage;
        [SetUp]
        public void Construct2()
        {
            MessageList = new Dictionary<int, string>();
            Networkstream = new Mock<FakeNetworkStreamInterface>();
            Tcpclient = new Mock<TcpClient>();
            RequestContext = new Mock<RequestContextInterface>();
            HTTPresponsewrapper = new Mock<HTTPResponseWrapperInterface>();
            MessageListMutex = new Mutex();         

            HTTPresponsewrapper.Setup(_ => _.SendDefaultStatus(It.IsAny<string>())).Returns(true).Callback((string y) => { status = y; });
            HTTPresponsewrapper.Setup(_ => _.SendDefaultMessage(It.IsAny<string>(), It.IsAny<string>())).Returns(true).Callback((string y, string z) => { status = y; message = z; });
               
            RequestContext.Setup(_ => _.Stream).Returns(Networkstream.Object);
            RequestContext.Setup(_ => _.ReponseHandler).Returns(HTTPresponsewrapper.Object);
            RequestContext.Setup(_ => _.ResolveEndPointToStringArray()).Returns(() => { return RequestContext.Object.MessageEndPoint.Split('/'); });
            status = "";
            message = "";
            MessageList.Add(0, "test");
            MessageList.Add(1, "abc");

            Messagecounter = MessageList.Count;
            Storage = new MessageStorageApi(ref MessageList, ref Messagecounter, ref MessageListMutex);
            EndpointCreator = new RegisteredEndpoints(ref Storage);
            EndPointController = new EndPointApi<RequestContextInterface, int>();
            EndpointCreator.ChainRegisterEndpoints(ref EndPointController);
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
                 
            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, revokestatuscode);
        }

        [Test]
        public void testEndPoint_get_404_emptyrequest()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(-2, revokestatuscode);
        }
        [Test]
        public void testEndPoint_get_404_noleadingslash()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "messages");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(-2, revokestatuscode);
        }
        [Test]
        public void testEndPoint_get_200_normalrequesttospecificendpoint()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "/messages/0");
            
            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, revokestatuscode);
        }
        [Test]
        public void testEndPoint_get_404_messagedoesnotexist()
        {
            SetupMockRequestContext("GET", "HTTP1.0", "/messages/3");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(404, revokestatuscode);
        }
        [Test]
        public void testEndPoint_Post_404_posttoinvalidendpoint()
        {
            SetupMockRequestContext("POST", "HTTP1.0", "/messages/3");

            revokestatuscode =EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(-3, revokestatuscode);
        }
        [Test]
        public void testEndPoint_Post_201_postcreated()
        {
            SetupMockRequestContext("POST", "HTTP1.0", "/messages", "PostTest");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual("201", status);
            Assert.AreEqual("PostTest", MessageList[2]);
            Assert.AreEqual(201, revokestatuscode);
        }
        [Test]
        public void testEndPoint_Post_404_invalidendpointalphanumeric()
        {
            SetupMockRequestContext("POST", "HTTP1.0", "/messages/a", "PostTest");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(-2, revokestatuscode);
        }
        [Test]
        public void testEndPoint_PUT_200_validputrequest()
        {
            SetupMockRequestContext("PUT", "HTTP1.0", "/messages/0", "PUTTEST");

 
            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, revokestatuscode);
            Assert.AreEqual("PUTTEST", MessageList[0]);
        }
        [Test]
        public void testEndPoint_PUT_404_invalidendpointalphanumeric()
        {
            SetupMockRequestContext("PUT", "HTTP1.0", "/messages/a", "PUTTEST");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(-2, revokestatuscode);
        }
        [Test]
        public void testEndPoint_PUT_404_messagedoesnotexist()
        {
            SetupMockRequestContext("PUT", "HTTP1.0", "/messages/5", "PUTTEST");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(404, revokestatuscode);
        }
        [Test]
        public void testEndPoint_Delete_404_invalidendpoint()
        {
            SetupMockRequestContext("DELETE", "HTTP1.0", "/messages");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(-3, revokestatuscode);
        }
        [Test]
        public void testEndPoint_Delete_404_doesnotexist()
        {
            SetupMockRequestContext("DELETE", "HTTP1.0", "/messages/5");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(404, revokestatuscode);
        }
        [Test]
        public void testEndPoint_Delete_404_successfuldelete()
        {
            SetupMockRequestContext("DELETE", "HTTP1.0", "/messages/0");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(200, revokestatuscode);
        }
        [Test]
        public void testEndPoint_unknownhttpverb_501()
        {
            SetupMockRequestContext("Hans", "HTTP1.0", "/messages/0");

            revokestatuscode = EndPointController.InvokeEndPoint(RequestContext.Object.HTTPVerb, RequestContext.Object.MessageEndPoint, RequestContext.Object);
            Assert.AreEqual(-3, revokestatuscode);
        }
        [Test]
        public void testParse()
        {
            Mock<FakeNetworkStreamInterface> MyFakeStream = new Mock<FakeNetworkStreamInterface>();
            Byte[] temparray = new byte[2000];
            string teststring = "GET /messages HTTP/1.1\r\nHost:127.0.0.1\r\nTest:Test\r\n\r\nPayloadTest";
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(teststring), temparray, teststring.Length);
               
            MyFakeStream.Setup(_ => _.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns((byte[] x, int y, int z) => { Array.Copy(temparray,0,x,0,teststring.Length); return teststring.Length; });            
            MyFakeStream.Setup(_ => _.DataAvailable).Returns(false);
            
            RequestContext ContextTest = new RequestContext(Tcpclient.Object,MyFakeStream.Object);
            ContextTest.Parse();
            string Key1 = ContextTest.SearchInDictionaryByKey("Host");
            string Key2 = ContextTest.SearchInDictionaryByKey("Test");

            Assert.AreEqual("GET", ContextTest.HTTPVerb);
            Assert.AreEqual("/messages", ContextTest.MessageEndPoint);
            Assert.AreEqual("HTTP/1.1", ContextTest.HttpProtokoll);
            Assert.AreEqual("PayloadTest", ContextTest.PayLoad);
            Assert.AreEqual("127.0.0.1", Key1);
            Assert.AreEqual("Test", Key2);          
        }
    }
}
