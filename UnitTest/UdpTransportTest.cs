using System;
using System.Net;
using Gelf4NLog.Target;
using Gelf4NLog.UnitTest.Resources;
using Moq;
using NLog;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Gelf4NLog.UnitTest
{
    public class UdpTransportTest
    {
        [TestFixture]
        public class SendMethod
        {
            [Test]
            public void ShouldSendShortUdpMessage()
            {
                var transportClient = new Mock<ITransportClient>();
                var transport = new UdpTransport(transportClient.Object);
                var converter = new Mock<IConverter>();
                converter.Setup(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>())).Returns(new JObject());

                var target = new NLogTarget(transport, converter.Object) { HostIp = "127.0.0.1" };
                var logEventInfo = new LogEventInfo { Message = "Test Message" };

                target.WriteLogEventInfo(logEventInfo);

                transportClient.Verify(t => t.Send(It.IsAny<byte[]>(), It.IsAny<Int32>(), It.IsAny<IPEndPoint>()), Times.Once());
                converter.Verify(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>()), Times.Once());
            }

            [Test]
            public void ShouldSendLongUdpMessage()
            {
                var jsonObject = new JObject();
                var message = ResourceHelper.GetResource("LongMessage.txt").ReadToEnd();
                
                jsonObject.Add("full_message", JToken.FromObject(message));

                var converter = new Mock<IConverter>();
                converter.Setup(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>())).Returns(jsonObject).Verifiable();
                var transportClient = new Mock<ITransportClient>();
                transportClient.Setup(t => t.Send(It.IsAny<byte[]>(), It.IsAny<Int32>(), It.IsAny<IPEndPoint>())).Verifiable();

                var transport = new UdpTransport(transportClient.Object);

                var target = new NLogTarget(transport, converter.Object) { HostIp = "127.0.0.1" };
                target.WriteLogEventInfo(new LogEventInfo());

                transportClient.Verify(t => t.Send(It.IsAny<byte[]>(), It.IsAny<Int32>(), It.IsAny<IPEndPoint>()), Times.Exactly(4));
                converter.Verify(c => c.GetGelfJson(It.IsAny<LogEventInfo>(), It.IsAny<string>()), Times.Once());
            }
        }
        
    }
}
