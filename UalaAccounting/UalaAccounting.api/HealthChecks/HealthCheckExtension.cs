using System;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace UalaAccounting.api.HealthChecks
{
    public static class HealthCheckExtension
    {
        public static WebApplication UseHealthCheck(this WebApplication app)
        {
            app.MapHealthChecks("health", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            return app;
        }
    }
}

