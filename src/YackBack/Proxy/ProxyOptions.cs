using System;
using System.IO;

namespace YackBack.Proxy
{
    public class ProxyOptions
    {
        public DirectoryInfo CacheDirectory { get; set; }

        public Uri ApiBaseAddress { get; set; }

        public void EnsureValid()
        {
            if (CacheDirectory == null)
                throw new InvalidOperationException("Invalid proxy options: CacheDirectory is null.");

            if (ApiBaseAddress == null)
                throw new InvalidOperationException("Invalid proxy options: ApiBaseAddress is null.");

            if (!ApiBaseAddress.IsAbsoluteUri)
                throw new InvalidOperationException("Invalid proxy options: ApiBaseAddress is a relative URI.");
        }
    }
}