using System;
using System.Linq;
using WebSpider.Abstractions;

namespace WebSpider.Core
{
    public class Spider
    {
        private readonly Cache _cache;

        private readonly SpiderQueue _queue;

        public Spider()
            : this(new Cache())
        {
        }

        public Spider(Cache cache)
        {
            _cache = cache;
            _queue = new SpiderQueue(PageRecieved);
        }

        public event Action OnCompleted;

        public event Action<Page> OnPageDownloaded;

        public IFilter Filter { get; set; }

        public void Enqueue(Uri url)
        {
            if (_cache.Contents.Contains(url) == false)
            {
                _cache.Add(url);

                var fetcher = new Downloader(url);
                _queue.Enqueue(fetcher);
            }
        }

        public void Start(Uri url = null)
        {
            _queue.OnCrawlingCompleted += () => OnCompleted?.Invoke();

            if (url != null)
                Enqueue(url);

            _queue.Process();
        }

        private void PageRecieved(Page page)
        {
            _cache.Add(page.Url);

            OnPageDownloaded?.Invoke(page);

            if (page.Html != string.Empty)
            {
                var links = page.RetrieveLinks();

                foreach (var link in links)
                {
                    if (_cache.Contents.Contains(link.Target))
                        continue;

                    if (Filter.CanBeExcluded(link.Target))
                        continue;

                    _queue.Enqueue(new Downloader(link.Target));
                }
            }
        }
    }
}
