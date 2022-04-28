using DevIO.Api.Extensions;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            services.AddHealthChecks()
                .AddElmahIoPublisher(options =>
                {
                    options.ApiKey = "b15427a11eae448f944aafbe57a8702e";
                    options.LogId = new Guid("1e4bd860-6029-4aca-b919-d66ff61a1061");
                    options.HeartbeatId = "66c2733653a24b999500a7e8b21b55fc";

                })
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            services.AddHealthChecksUI()
                .AddSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            // Logging provider config example
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

            app.UseHealthChecks("/api/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/api/hc-ui";
                options.ResourcesPath = $"{options.UIPath}/resources";
                options.UseRelativeApiPath = false;
                options.UseRelativeResourcesPath = false;
                options.UseRelativeWebhookPath = false;

            });

            return app;
        }
    }
}
