using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebSpider.Common;

namespace WebSpider.Core
{
    public class SpiderQueue
    {
        #region Fields

        private readonly object _thisLock = new object();

        private readonly IList<Downloader> _executing;

        private readonly IList<Downloader> _queue;

        private readonly Action<Page> _onPageDownloaded;

        #endregion Fields

        public SpiderQueue(Action<Page> onPageDownloaded)
        {
            _executing = new List<Downloader>();
            _queue = new List<Downloader>();
            _onPageDownloaded = onPageDownloaded;
        }

        public event Action OnCrawlingCompleted;

        public void Enqueue(Downloader downloader)
        {
            lock (_thisLock)
            {
                if (_queue.All(d => d.Uri.AbsoluteUri != downloader.Uri.AbsoluteUri))
                    _queue.Add(downloader);
            }
        }

        public void Process()
        {
            new Thread(enqueuerThreadMethod).Start();
            new Thread(downloaderCompletionCheckMethod).Start();

            void enqueuerThreadMethod()
            {
                var rand = new Random();

                while (true)
                {
                    lock (_thisLock)
                    {
                        if (_queue.Any() && _executing.Count() < Constants.MaxConcurrency)
                        {
                            var fetcher = _queue[rand.Next(_queue.Count)];

                            if (_executing.Any(d => d.Uri == fetcher.Uri))
                            {
                                _queue.Remove(fetcher);
                            }
                            else
                            {
                                _queue.Remove(fetcher);
                                _executing.Add(fetcher);
                                fetcher.StartFetch();
                            }
                        }
                    }
                }
            }

            void downloaderCompletionCheckMethod()
            {
                while (true)
                {
                    lock (_thisLock)
                    {
                        if (_executing.Any(d => d.Completed))
                        {
                            var downloader = _executing.First(d => d.Completed);
                            RemoveFetcher(downloader);

                            if (downloader.DownloadedPage != null)
                            {
                                _onPageDownloaded(downloader.DownloadedPage);

                                if (CheckForCompleteness() && OnCrawlingCompleted != null)
                                {
                                    OnCrawlingCompleted();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool CheckForCompleteness()
        {
            return _queue.Any() == false
                && _executing.Any() == false;
        }

        private void RemoveFetcher(Downloader downloader)
        {
            lock (_thisLock)
            {
                _executing.Remove(downloader);
                _queue.Remove(downloader);
            }
        }
    }
}
