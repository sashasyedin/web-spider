using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebSpider.Common;

namespace WebSpider.Core
{
    public class Page
    {
        public Page(Uri url, string html)
        {
            Url = url;
            Html = html;
        }

        #region Properties

        public Uri Url { get; }

        public string Html { get; }

        public string ContentType { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        #endregion Properties

        public IEnumerable<Link> RetrieveLinks()
        {
            var linkNodes = GetHtmlDocument().DocumentNode.SelectNodes(Constants.LinkXpath);

            if (linkNodes == null)
                return Enumerable.Empty<Link>();

            var links = new List<Link>();

            foreach (var linkNode in linkNodes)
            {
                var href = linkNode.GetAttributeValue(Constants.HrefAttribute, Constants.Hash);

                if (Uri.IsWellFormedUriString(href, UriKind.RelativeOrAbsolute) == false)
                    continue;

                var uri = new Uri(href, UriKind.RelativeOrAbsolute);

                if (uri.IsAbsoluteUri == false)
                    uri = new Uri(Url, href.Trim('/'));

                links.Add(new Link(uri));
            }

            return links;
        }

        private HtmlDocument GetHtmlDocument()
        {
            var document = new HtmlDocument();
            document.LoadHtml(Html);
            return document;
        }
    }
}
