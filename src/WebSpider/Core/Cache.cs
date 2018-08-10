using System;
using System.Collections;
using System.Collections.Generic;

namespace WebSpider.Core
{
    public class Cache
    {
        private readonly object _thisLock = new object();

        public Cache()
        {
            Contents = new HashSet<Uri>();
        }

        public Cache(Uri seedUrl)
            : this()
        {
            Add(seedUrl);
        }

        public HashSet<Uri> Contents { get; }

        public void Add(Uri url)
        {
            lock (_thisLock)
            {
                Contents.Add(url);
            }
        }

        public bool Exists(Uri url)
        {
            var exists = Contents.Contains(url);
            return exists;
        }
    }
}
