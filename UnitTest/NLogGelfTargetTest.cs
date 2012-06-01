using Gelf4NLog.Target;
using Moq;
using NUnit.Framework;

namespace Gelf4NLog.UnitTest
{
    public class NLogGelfTargetTest
    {
        [TestFixture]
        public class Constructor
        {
            [Test]
            public void ShouldSelfInitializeWithEmptyConstructor()
            {
                var target = new NLogGelfTarget();
                Assert.IsNotNull(target);
                Assert.IsNotNull(target.Converter);
                Assert.IsNotNull(target.Transport);
                Assert.AreEqual("Gelf4NLog.Target.GelfConverter", target.Converter.GetType().FullName);
                Assert.AreEqual("Gelf4NLog.Target.UdpTransport", target.Transport.GetType().FullName);
            }

            [Test]
            public void ShouldUsePassedObjects()
            {
                var converter = new Mock<IConverter>();
                var transport = new Mock<ITransport>();

                var target = new NLogGelfTarget(transport.Object, converter.Object);
                Assert.IsNotNull(target);
                Assert.IsNotNull(target.Converter);
                Assert.IsNotNull(target.Transport);
                Assert.AreEqual("Castle.Proxies.IConverterProxy", target.Converter.GetType().FullName);
                Assert.AreEqual("Castle.Proxies.ITransportProxy", target.Transport.GetType().FullName);
            }
        }
    }
}
