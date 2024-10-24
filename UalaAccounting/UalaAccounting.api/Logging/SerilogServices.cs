using System;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;

namespace UalaAccounting.api.Logging
{
    public static class SerilogServices
    {
        public static IServiceCollection AddSerilogServices(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console(new JsonFormatter());

                var logger = loggerConfiguration.CreateLogger();

                builder.Services.AddSingleton<ILoggerFactory>(
                    provider => new SerilogLoggerFactory(logger, dispose: false)
                    );
            });

            return services;
        }
    }
}

