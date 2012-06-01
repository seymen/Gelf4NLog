namespace Gelf4NLog.Target
{
    public interface ITransport
    {
        void Send(string serverIpAddress, int serverPort, string message);
    }
}
