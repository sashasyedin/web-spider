using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WebSpider.Abstractions;

namespace WebSpider.Client
{
    public class Filter : IFilter
    {
        #region Constants

        private const string Hash = "#";

        private const string ImgPattern = @"(\.jpg|\.css|\.js|\.gif|\.jpeg|\.png|\.ico)";

        private const string Javascript = "javascript";

        #endregion Constants

        private readonly Regex _regex = new Regex(ImgPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public Filter(params string[] hosts)
        {
            Hosts = hosts;
        }

        #region Properties

        public IEnumerable<string> Hosts { get; }

        public bool ExcludeAnchors { get; set; }

        public bool ExcludeImages { get; set; }

        public bool ExcludeJavaScript { get; set; }

        #endregion Properties

        bool IFilter.CanBeExcluded(Uri uri)
        {
            if (Hosts.Contains(uri.Host) == false)
                return true;

            if (ExcludeAnchors && uri.AbsoluteUri.Contains(Hash))
                return true;

            if (ExcludeImages && _regex.IsMatch(uri.AbsoluteUri))
                return true;

            if (ExcludeJavaScript && uri.Scheme.Equals(Javascript))
                return true;

            return false;
        }
    }
}
