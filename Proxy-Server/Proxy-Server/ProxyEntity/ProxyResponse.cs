namespace Proxy_Server.ProxyEntity
{
    public class ProxyResponse
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
    }
}
