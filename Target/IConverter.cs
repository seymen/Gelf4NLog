using NLog;
using Newtonsoft.Json.Linq;

namespace Gelf4NLog.Target
{
    public interface IConverter
    {
        JObject GetJsonObject(LogEventInfo logEventInfo, string facility);
    }
}
