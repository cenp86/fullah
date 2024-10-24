using System;
using System.Diagnostics;

namespace UalaAccounting.api.Logging.Enrichers
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _isDurationLoggingEnabled;
        private readonly ILogger<LoggerMiddleware> _logger;

        public LoggerMiddleware(RequestDelegate next, IConfiguration config, ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _isDurationLoggingEnabled = config.GetValue("EnableDurationLogging", true);
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                if (_isDurationLoggingEnabled)
                {
                    var stopwatch = Stopwatch.StartNew();
                    _logger.LogInformation($"Process started at {DateTime.Now.ToString("o")}");

                    await _next(httpContext);

                    stopwatch.Stop();
                    _logger.LogInformation($"Process ended at {DateTime.Now.ToString("o")}. Total duration: {stopwatch.Elapsed.Minutes} minutes and " +
                        $"{stopwatch.Elapsed.Seconds} seconds.");
                }
                else await _next(httpContext);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during the proccess: {ex}");
                throw;
            }
        }
    }

    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseWhen(context => !context.Request.Path.Equals("/health"), appBuilder =>
            {
                appBuilder.UseMiddleware<LoggerMiddleware>();
            });
        }
    }
}

