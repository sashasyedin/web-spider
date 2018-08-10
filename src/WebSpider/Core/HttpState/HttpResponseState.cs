using System.IO;
using System.Net;

namespace WebSpider.Core.HttpState
{
    public class HttpResponseState
    {
        public HttpResponseState(WebResponse webResponse, byte[] buffer, MemoryStream outStream)
        {
            Buffer = buffer;
            OutStream = outStream;
            WebResponse = webResponse;
        }

        public byte[] Buffer { get; }

        public MemoryStream OutStream { get; }

        public WebResponse WebResponse { get; }
    }
}
