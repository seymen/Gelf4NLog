using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using NLog;
using Newtonsoft.Json.Linq;

namespace Gelf4NLog.Target
{
    public class GelfConverter : IConverter
    {
        private const int ShortMessageMaxLength = 250;
        private const string GelfVersion = "1.0";

        public JObject GetJsonObject(LogEventInfo logEventInfo, string facility)
        {
            //Add logger name as additional property
            logEventInfo.Properties.Add("_loggerName", logEventInfo.LoggerName);

            var message = logEventInfo.FormattedMessage;
            
            if (logEventInfo.Exception != null)
            {
                message = String.Format(CultureInfo.InvariantCulture, "{0} - {1}. {2}. {3}.", 
                    message, 
                    logEventInfo.Exception.Source, 
                    logEventInfo.Exception.Message, 
                    logEventInfo.Exception.StackTrace);
            }

            var shortMessage = message;
            if (shortMessage.Length > ShortMessageMaxLength)
            {
                shortMessage = shortMessage.Substring(0, ShortMessageMaxLength);
            }

            var gelfMessage = new GelfMessage
            {
                Facility = (facility ?? "GELF"),
                File = (logEventInfo.UserStackFrame != null) ? logEventInfo.UserStackFrame.GetFileName() : string.Empty,
                FullMessage = message,
                Host = Dns.GetHostName(),
                Level = GetSeverityLevel(logEventInfo.Level),
                Line = (logEventInfo.UserStackFrame != null) ? logEventInfo.UserStackFrame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture) : string.Empty,
                ShortMessage = shortMessage,
                Timestamp = logEventInfo.TimeStamp,
                Version = GelfVersion
            };

            var jsonObject = JObject.FromObject(gelfMessage);
            
            if (logEventInfo.Properties != null)
            {
                foreach (var property in logEventInfo.Properties)
                {
                    var key = property.Key as string;
                    var value = property.Value as string;
                    AddAdditionalField(jsonObject, key, value);
                }
            }

            return jsonObject;
        }

        private static void AddAdditionalField(IDictionary<string, JToken> jObject, string key, string value)
        {
            if (key == null) return;

            //According to the GELF spec, libraries should NOT allow to send id as additional field (_id)
            //server MUST skip the field because it could override the MongoDB _key field
            if (key.Equals("id", StringComparison.OrdinalIgnoreCase))
                key = "id_";

            //According to the GELF spec, keys should start with '_' to avoid collision
            if (!key.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                key = "_" + key;

            jObject.Add(key, value);
        }

        /// <summary>
        /// Values from SyslogSeverity enum here: http://marc.info/?l=log4net-dev&m=109519564630799
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static int GetSeverityLevel(LogLevel level)
        {
            if (level == LogLevel.Debug)
            {
                return 7;
            }
            if (level == LogLevel.Fatal)
            {
                return 2;
            }
            if (level == LogLevel.Info)
            {
                return 6;
            }
            if (level == LogLevel.Trace)
            {
                return 6;
            }
            if (level == LogLevel.Warn)
            {
                return 4;
            }

            return 3; //LogLevel.Error
        }
    }
}
