using CommandLine;
using System;
using System.IO;
using WebSpider.Core;

namespace WebSpider.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<Options>(args)
                .WithParsed(opts => Run(opts));
        }

        private static void Run(Options options)
        {
            var filter = new Filter(options.Host)
            {
                ExcludeAnchors = options.ExcludeAnchors,
                ExcludeImages = options.ExcludeImages,
                ExcludeJavaScript = options.ExcludeJavaScript
            };

            var crawler = new Spider { Filter = filter };

            crawler.OnCompleted += () =>
            {
                Console.WriteLine("Completed");
                Console.ReadKey();
            };

            crawler.OnPageDownloaded += page =>
            {
                File.WriteAllText($@"{options.Target}\{Path.GetFileName(page.Url.LocalPath)}_{Guid.NewGuid()}.html", page.Html);
                Console.WriteLine($"Downloaded: {page.Url}");
            };

            crawler.Enqueue(new Uri($"http://{options.Host}/"));
            crawler.Start();

            Console.WriteLine("Started.");
            Console.ReadLine();
        }

        private class Options
        {
            [Option('h', "host", Required = true, HelpText = "Host")]
            public string Host { get; set; }

            [Option('t', "targetFolder", Required = false, HelpText = "Target folder")]
            public string Target { get; set; }

            [Option('a', "excludeAnchors", Required = false, HelpText = "Exclude anchors")]
            public bool ExcludeAnchors { get; set; }

            [Option('i', "excludeImages", Required = false, HelpText = "Exclude images")]
            public bool ExcludeImages { get; set; }

            [Option('j', "excludeJavaScript", Required = false, HelpText = "Exclude javascript")]
            public bool ExcludeJavaScript { get; set; }
        }
    }
}
