using System;
using System.Net;
using Gelf4NLog.Target;
using NLog;
using NUnit.Framework;

namespace Gelf4NLog.UnitTest
{
    public class GelfConverterTest
    {
        [TestFixture(Category = "GelfConverter")]
        public class GetGelfJsonMethod
        {
            [Test]
            public void ShouldCreateGelfJsonCorrectly()
            {
                var timestamp = DateTime.Now;
                var logEvent = new LogEventInfo
                                   {
                                       Message = "Test Log Message", 
                                       Level = LogLevel.Info, 
                                       TimeStamp = timestamp,
                                       LoggerName = "GelfConverterTestLogger"
                                   };
                logEvent.Properties.Add("customproperty1", "customvalue1");
                logEvent.Properties.Add("customproperty2", "customvalue2");

                var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility");

                Assert.IsNotNull(jsonObject);
                Assert.AreEqual("1.0", jsonObject.Value<string>("version"));
                Assert.AreEqual(Dns.GetHostName(), jsonObject.Value<string>("host"));
                Assert.AreEqual("Test Log Message", jsonObject.Value<string>("short_message"));
                Assert.AreEqual("Test Log Message", jsonObject.Value<string>("full_message"));
                Assert.AreEqual(timestamp, jsonObject.Value<DateTime>("timestamp"));
                Assert.AreEqual(6, jsonObject.Value<int>("level"));
                Assert.AreEqual("TestFacility", jsonObject.Value<string>("facility"));
                Assert.AreEqual("", jsonObject.Value<string>("file"));
                Assert.AreEqual("", jsonObject.Value<string>("line"));
                
                Assert.AreEqual("customvalue1", jsonObject.Value<string>("_customproperty1"));
                Assert.AreEqual("customvalue2", jsonObject.Value<string>("_customproperty2"));
                Assert.AreEqual("GelfConverterTestLogger", jsonObject.Value<string>("_LoggerName"));
                
                //make sure that there are no other junk in there
                Assert.AreEqual(12, jsonObject.Count);
            }

            [Test]
            public void ShouldHandleExceptionsCorrectly()
            {
                var logEvent = new LogEventInfo
                                   {
                                       Message = "Test Message",
                                       Exception = new DivideByZeroException("div by 0")
                                   };

                var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility");

                Assert.IsNotNull(jsonObject);
                Assert.AreEqual("Test Message", jsonObject.Value<string>("short_message"));
                Assert.AreEqual("Test Message", jsonObject.Value<string>("full_message"));
                Assert.AreEqual(3, jsonObject.Value<int>("level"));
                Assert.AreEqual("TestFacility", jsonObject.Value<string>("facility"));
                Assert.AreEqual(null, jsonObject.Value<string>("_ExceptionSource"));
                Assert.AreEqual("div by 0", jsonObject.Value<string>("_ExceptionMessage"));
                Assert.AreEqual(null, jsonObject.Value<string>("_StackTrace"));
                Assert.AreEqual(null, jsonObject.Value<string>("_LoggerName"));
            }

            [Test]
            public void ShouldHandleLongMessageCorrectly()
            {
                var logEvent = new LogEventInfo
                {
                    //The first 300 chars of lorem ipsum...
                    Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus interdum est in est cursus vitae pellentesque felis lobortis. Donec a orci quis ante viverra eleifend ac et quam. Donec imperdiet libero ut justo tincidunt non tristique mauris gravida. Fusce sapien eros, tincidunt a placerat nullam."
                };

                var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility");

                Assert.IsNotNull(jsonObject);
                Assert.AreEqual(250, jsonObject.Value<string>("short_message").Length);
                Assert.AreEqual(300, jsonObject.Value<string>("full_message").Length);
            }

            [Test]
            public void ShouldHandlePropertyCalledIdProperly()
            {
                var logEvent = new LogEventInfo { Message = "Test" };
                logEvent.Properties.Add("Id", "not_important");

                var jsonObject = new GelfConverter().GetGelfJson(logEvent, "TestFacility");

                Assert.IsNotNull(jsonObject);
                Assert.IsNull(jsonObject["_id"]);
                Assert.AreEqual("not_important", jsonObject.Value<string>("_id_"));
            }

            [TestCase("")]
            [TestCase(null)]
            public void ShouldSetDefaultFacility(string facility)
            {
                var logEvent = new LogEventInfo {Message = "Test"};

                var jsonObject = new GelfConverter().GetGelfJson(logEvent, facility);

                Assert.AreEqual("GELF", jsonObject.Value<string>("facility"));
            }
        }
    }
}
