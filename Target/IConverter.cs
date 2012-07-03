using NLog;
using Newtonsoft.Json.Linq;

namespace Gelf4NLog.Target
{
    public interface IConverter
    {
        JObject GetGelfJson(LogEventInfo logEventInfo, string facility);
    }
}
