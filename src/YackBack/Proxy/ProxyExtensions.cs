using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace YackBack.Proxy
{
    public static class ProxyExtensions
    {
        public static IServiceCollection AddProxy(this IServiceCollection services, ProxyOptions options)
        {
            return services.AddSingleton<ProxyOptions>(options);
        }

        public static IApplicationBuilder UseProxy(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ProxyMiddleware>();
        }
    }
}