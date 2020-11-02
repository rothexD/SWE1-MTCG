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
        public string message;
        public int counter;
        public Dictionary<int, string> MessageList;

        [SetUp]
        public void Construct2()
        {
            HTTPresponsewrapper = new Mock<HTTPResponseWrapperInterface>();
            Networkstream = new Mock<FakeNetworkStreamInterface>();
            Tcpclient = new Mock<TcpClient>();
            RequestContext = new Mock<RequestContextInterface>();

            HTTPresponsewrapper.Setup(_ => _.SendDefaultStatus(It.IsAny<FakeNetworkStreamInterface>(), It.IsAny<string>())).Returns(true).Callback((FakeNetworkStreamInterface x, string y) => { status = y; });
            HTTPresponsewrapper.Setup(_ => _.SendDefaultMessage(It.IsAny<FakeNetworkStreamInterface>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true).Callback((FakeNetworkStreamInterface x, string y, string z) => { status = y; message = z; });
            RequestContext.Setup(_ => _.ResolveEndPointToStringArray()).Returns(() => { return RequestContext.Object.MessageEndPoint.Split('/'); });
            status = "";
            message = "";       
            MessageList = new Dictionary<int, string>();
            MessageList.Add(0, "test");
            MessageList.Add(1, "abc");
            counter = MessageList.Count;
        }
        [Test]
        public void testEndPoint_get_200_normalrequest()
        {            
            RequestContext.Setup(_ => _.HTTPVerb).Returns("GET");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages");       
        
            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("200", status);
        }

        [Test]
        public void testEndPoint_get_404_emptyrequest()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("GET");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_get_404_noleadingslash()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("GET");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("messages");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_get_200_normalrequesttospecificendpoint()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("GET");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/0");
            
            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("200", status);
        }
        [Test]
        public void testEndPoint_get_404_messagedoesnotexist()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("GET");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/3");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_Post_404_posttoinvalidendpoint()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("POST");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/3");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_Post_201_postcreated()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("POST");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages");
            RequestContext.Setup(_ => _.PayLoad).Returns("PostTest");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("201", status);
            Assert.AreEqual("PostTest", MessageList[2]);
            Assert.AreEqual("2", message);
        }
        [Test]
        public void testEndPoint_Post_404_invalidendpointalphanumeric()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("POST");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/a");
            RequestContext.Setup(_ => _.PayLoad).Returns("PostTest");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_PUT_200_validputrequest()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("PUT");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/0");
            RequestContext.Setup(_ => _.PayLoad).Returns("PUTTEST");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("200", status);
            Assert.AreEqual("PUTTEST", MessageList[0]);
        }
        [Test]
        public void testEndPoint_PUT_404_invalidendpointalphanumeric()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("PUT");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/a");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("400", status);
        }
        [Test]
        public void testEndPoint_PUT_404_messagedoesnotexist()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("PUT");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/5");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_Delete_404_invalidendpoint()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("DELETE");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_Delete_404_doesnotexist()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("DELETE");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/5");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("404", status);
        }
        [Test]
        public void testEndPoint_Delete_404_successfuldelete()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("DELETE");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/0");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("200", status);
        }
        [Test]
        public void testEndPoint_unknownhttpverb_501()
        {
            RequestContext.Setup(_ => _.HTTPVerb).Returns("Hans");
            RequestContext.Setup(_ => _.HttpProtokoll).Returns("HTTP1.0");
            RequestContext.Setup(_ => _.MessageEndPoint).Returns("/messages/0");

            Program.MessageListMutex = new Mutex();
            Program.endpointMessage(HTTPresponsewrapper.Object, RequestContext.Object, ref MessageList, ref counter);
            Assert.AreEqual("501", status);
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
