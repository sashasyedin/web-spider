using System;
using System.IO;
using System.Net;
using System.Text;
using WebSpider.Common;
using WebSpider.Core.HttpState;

namespace WebSpider.Core
{
    public class Downloader
    {
        public Downloader(Uri uri)
        {
            Uri = uri;
        }

        #region Properties

        public Uri Uri { get; }

        public bool Completed { get; private set; }

        public Page DownloadedPage { get; private set; }

        #endregion Properties

        public void StartFetch()
        {
            var webRequest = WebRequest.Create(Uri);
            webRequest.BeginGetResponse(EndGetResponse, new HttpRequestState(webRequest));
        }

        private void EndGetResponse(IAsyncResult ar)
        {
            var httpRequestState = ar.AsyncState as HttpRequestState;
            var webRequest = httpRequestState.WebRequest;

            try
            {
                var webResponse = webRequest.EndGetResponse(ar);
                var responseStream = webResponse.GetResponseStream();
                var outStream = new MemoryStream();
                var buffer = new byte[Constants.BufferSize];
                var state = new HttpResponseState(webResponse, buffer, outStream);

                responseStream.BeginRead(buffer, 0, Constants.BufferSize, EndRead, state);
            }
            catch (WebException e)
            {
                if (e.Response is HttpWebResponse response)
                {
                    DownloadedPage = new Page(Uri, string.Empty)
                    {
                        StatusCode = response.StatusCode,
                        ContentType = response.ContentType
                    };
                }

                Completed = true;
            }
        }

        private void EndRead(IAsyncResult ar)
        {
            var httpResponseState = ar.AsyncState as HttpResponseState;
            var bytesRecieved = httpResponseState.WebResponse.GetResponseStream().EndRead(ar);
            var outStream = httpResponseState.OutStream;
            var responseStream = httpResponseState.WebResponse.GetResponseStream();

            if (bytesRecieved > 0)
            {
                outStream.Write(httpResponseState.Buffer, 0, bytesRecieved);
                responseStream.BeginRead(httpResponseState.Buffer, 0, Constants.BufferSize, EndRead, httpResponseState);
            }
            else
            {
                responseStream.Close();
                httpResponseState.WebResponse.Close();

                if (outStream.Length > 0)
                {
                    var buffer = outStream.ToArray();
                    var html = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    var httpWebResponse = (HttpWebResponse)httpResponseState.WebResponse;

                    DownloadedPage = new Page(Uri, html)
                    {
                        Headers = httpWebResponse.Headers,
                        StatusCode = httpWebResponse.StatusCode,
                        ContentType = httpWebResponse.ContentType
                    };
                }

                Completed = true;
            }
        }
    }
}
