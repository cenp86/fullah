using System;
using Serilog.Core;
using Serilog.Events;

namespace UalaAccounting.api.Logging.Enrichers
{
    public class TracingAttributesEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var spanId = propertyFactory.CreateProperty("spanId", "");
            logEvent.AddPropertyIfAbsent(spanId);
        }
    }
}

