using System.ComponentModel.DataAnnotations;
using NLog;
using NLog.Targets;
using Newtonsoft.Json;

namespace Gelf4NLog.Target
{
    [Target("Gelf")]
    public class NLogGelfTarget : TargetWithLayout
    {
        [Required]
        public string HostIp { get; set; }

        [Required]
        public int HostPort { get; set; }

        public string Facility { get; set; }

        public IConverter Converter { get; private set; }
        public ITransport Transport { get; private set; }

        public NLogGelfTarget()
        {
            Transport = new UdpTransport(new UdpTransportClient());
            Converter = new GelfConverter();
        }

        public NLogGelfTarget(ITransport transport, IConverter converter)
        {
            Transport = transport;
            Converter = converter;
        }

        public void WriteLogEventInfo(LogEventInfo logEvent)
        {
            Write(logEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var jsonObject = Converter.GetJsonObject(logEvent, Facility);
            Transport.Send(HostIp, HostPort, jsonObject.ToString(Formatting.None, null));
        }
    }
}
