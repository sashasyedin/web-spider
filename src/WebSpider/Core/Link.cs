using System;

namespace WebSpider.Core
{
    public class Link
    {
        public Link(Uri target)
        {
            Target = target;
        }

        public Uri Target { get; }
    }
}
