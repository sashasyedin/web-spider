using System.Net;

namespace WebSpider.Core.HttpState
{
    public class HttpRequestState
    {
        public HttpRequestState(WebRequest webRequest)
        {
            WebRequest = webRequest;
        }

        public WebRequest WebRequest { get; }
    }
}
