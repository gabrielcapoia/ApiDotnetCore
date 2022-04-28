using Elmah.Io.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "b15427a11eae448f944aafbe57a8702e";
                o.LogId = new Guid("1e4bd860-6029-4aca-b919-d66ff61a1061");
            });

            //services.AddLogging(builder =>
            //{

            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "b15427a11eae448f944aafbe57a8702e";
            //        o.LogId = new Guid("1e4bd860-6029-4aca-b919-d66ff61a1061");
            //    });

            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //});

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();
            return app;
        }
    }
}
