using System;
using System.Collections.Generic;

namespace WebSpider.Abstractions
{
    public interface IFilter
    {
        bool CanBeExcluded(Uri uri);

        IEnumerable<string> Hosts { get; }
    }
}
