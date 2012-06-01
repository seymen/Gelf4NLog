using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Gelf4NLog.Target;
using NLog;
using NUnit.Framework;

namespace Gelf4NLog.UnitTest
{
    public class GelfConverterTest
    {
        [TestFixture(Category = "GelfConverter")]
        public class GetJsonObjectMethod
        {
            [Test]
            public void ShouldSerializeLogEventCorrectly()
            {
                var timestamp = new DateTime(2012,02,28);
                var logEvent = new LogEventInfo
                                   {
                                       Message = "Test Log Message", 
                                       Level = LogLevel.Debug, 
                                       TimeStamp = timestamp,
                                       LoggerName = "GelfConverterTest"
                                   };
                logEvent.Properties.Add("customproperty1", "customvalue1");
                logEvent.Properties.Add("customproperty2", "customvalue2");
                var jsonObject = new GelfConverter().GetJsonObject(logEvent, "TestFacility");

                Assert.IsNotNull(jsonObject);
                Assert.AreEqual("TestFacility", jsonObject.Value<string>("facility"));
                Assert.AreEqual("", jsonObject.Value<string>("file"));
                Assert.AreEqual("Test Log Message", jsonObject.Value<string>("full_message"));
                Assert.AreEqual(Dns.GetHostName(), jsonObject.Value<string>("host"));
                Assert.AreEqual(7, jsonObject.Value<int>("level"));
                Assert.AreEqual("", jsonObject.Value<string>("line"));
                Assert.AreEqual("Test Log Message", jsonObject.Value<string>("short_message"));
                Assert.AreEqual(new DateTime(2012,02,28), jsonObject.Value<DateTime>("timestamp"));
                Assert.AreEqual("1.0", jsonObject.Value<string>("version"));
                Assert.AreEqual("customvalue1", jsonObject.Value<string>("_customproperty1"));
                Assert.AreEqual("customvalue2", jsonObject.Value<string>("_customproperty2"));
                Assert.AreEqual("GelfConverterTest", jsonObject.Value<string>("_loggerName"));
            }

            [Test]
            public void ShouldSerializeLongMessageCorrectly()
            {
                var timestamp = new DateTime(2012, 02, 28);
                var logEvent = new LogEventInfo
                {
                    Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus interdum est in est cursus vitae pellentesque felis lobortis. Donec a orci quis ante viverra eleifend ac et quam. Donec imperdiet libero ut justo tincidunt non tristique mauris gravida. Fusce sapien eros, tincidunt a placerat nullam.",
                    Level = LogLevel.Debug,
                    TimeStamp = timestamp
                };
                var jsonObject = new GelfConverter().GetJsonObject(logEvent, "TestFacility");

                Assert.IsNotNull(jsonObject);
                Assert.AreEqual(250, jsonObject.Value<string>("short_message").Length);
                Assert.AreEqual(300, jsonObject.Value<string>("full_message").Length);
            }

            [Test]
            public void ShouldConvertLogEventWithPropertyNamedIdCorrectly()
            {
                var logEvent = new LogEventInfo();
                logEvent.Properties.Add("Id", "123");
                logEvent.Message = "Test Message";
                var jsonObject = new GelfConverter().GetJsonObject(logEvent, "TestFacility");
                Assert.IsNotNull(jsonObject);
                Assert.IsNull(jsonObject["_id"]);
                Assert.AreEqual("123", jsonObject.Value<string>("_id_"));
            }

            [Test]
            public void ShouldConvertExceptionCorrectly()
            {
                var logEvent = new LogEventInfo
                                   {
                                       Message = "Test Message",
                                       Exception = new InvalidDataException("Your data is all wrong...")
                                   };

                var jsonObject = new GelfConverter().GetJsonObject(logEvent, "TestFacility");
                Assert.IsNotNull(jsonObject);
                Assert.AreEqual("Test Message - . Your data is all wrong.... .", jsonObject.Value<string>("short_message"));
                Assert.AreEqual("Test Message - . Your data is all wrong.... .", jsonObject.Value<string>("full_message"));
            }

            [Test]
            public void ShouldConvertLogLevelsCorrectly()
            {
                var logLevels = new List<LogLevel>
                                    {
                                        LogLevel.Debug,
                                        LogLevel.Error,
                                        LogLevel.Fatal,
                                        LogLevel.Info,
                                        LogLevel.Trace,
                                        LogLevel.Warn
                                    };

                foreach (var logLevel in logLevels)
                {
                    var logEvent = new LogEventInfo
                                       {
                                           Message = "LogLevelTest",
                                           Level = logLevel
                                       };
                    var jsonObject = new GelfConverter().GetJsonObject(logEvent, "TestFacility");
                    Assert.IsNotNull(jsonObject);
                    jsonObject.Value<Int32>("level");
                }
            }
        }
    }
}
