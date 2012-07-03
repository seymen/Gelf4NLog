using System.ComponentModel.DataAnnotations;
using NLog;
using NLog.Targets;
using Newtonsoft.Json;

namespace Gelf4NLog.Target
{
    [Target("GrayLog")]
    public class NLogTarget : TargetWithLayout
    {
        [Required]
        public string HostIp { get; set; }

        [Required]
        public int HostPort { get; set; }

        public string Facility { get; set; }

        public IConverter Converter { get; private set; }
        public ITransport Transport { get; private set; }

        public NLogTarget()
        {
            Transport = new UdpTransport(new UdpTransportClient());
            Converter = new GelfConverter();
        }

        public NLogTarget(ITransport transport, IConverter converter)
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
            var jsonObject = Converter.GetGelfJson(logEvent, Facility);
            if (jsonObject == null) return;
            Transport.Send(HostIp, HostPort, jsonObject.ToString(Formatting.None, null));
        }
    }
}
