using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace YackBack
{
    using Proxy;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            string[] commandLineArguments = Environment.GetCommandLineArgs().Skip(1).ToArray();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddCommandLine(commandLineArguments)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddProxy(new ProxyOptions
            {
                CacheDirectory = new DirectoryInfo(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        Configuration["Proxy:Cache:Directory"]
                    )
                ),
                ApiBaseAddress = new Uri(
                    Configuration["Proxy:ApiBaseAddress"]
                )
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseProxy();
        }
    }
}