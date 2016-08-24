using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YackBack.Proxy
{
    public class ProxyMiddleware
    {
        readonly RequestDelegate             _next;
        readonly DirectoryInfo              _cacheDirectory;
        readonly Uri                        _apiBaseAddress;

        public ProxyMiddleware(RequestDelegate next, ILogger<ProxyMiddleware> logger, ProxyOptions options)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _next = next;
            Log = logger;

            options.EnsureValid();
            _cacheDirectory = options.CacheDirectory;
            _apiBaseAddress = options.ApiBaseAddress;
        }

        ILogger Log { get; }

        public async Task Invoke(HttpContext context)
        {
            // TODO: Proxy call to API.
            Log.LogDebug("Received request for '{RequestPath}'. Corresponding cache path is '{CachePath}'.",
                context.Request.Path,
                GetCachePath(context)
            );

            Uri requestUri = GetProxyApiUri(context);
            
            Log.LogDebug("Request '{RequestPath}' will be passed through to the proxied API at '{ApiUri}'.",
                context.Request.Path,
                requestUri.AbsoluteUri
            );
            
            await _next(context);
        }

        string GetCachePath(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            StringBuilder cachePathBuilder =
                new StringBuilder(_cacheDirectory.FullName)
                    .Append(context.Request.Path)
                    .Replace('/', Path.DirectorySeparatorChar);

            if (context.Request.QueryString.HasValue)
            {
                cachePathBuilder.Append("__");

                // TODO: Query string is escaped, but is that sufficient?
                cachePathBuilder.Append(
                    context.Request.QueryString.Value
                        .Substring(1) // Skip the leading '?'
                );
            }

            return cachePathBuilder.ToString();
        }

        Uri GetProxyApiUri(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            UriBuilder requestUriBuilder = new UriBuilder(_apiBaseAddress);
            requestUriBuilder.Path += context.Request.Path.ToUriComponent().Substring(1); // Skip leading slash.
            if (context.Request.QueryString.HasValue)
                requestUriBuilder.Query = context.Request.QueryString.ToUriComponent();

            return requestUriBuilder.Uri;
        }
    }
}
