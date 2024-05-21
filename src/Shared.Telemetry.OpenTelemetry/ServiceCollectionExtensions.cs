using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Telemetry.OpenTelemetry
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, Action<OpenTelemetryConfiguration>? configure = null)
        {
            OpenTelemetryConfiguration configuration = new();
            configure?.Invoke(configuration);

            if (string.IsNullOrWhiteSpace(configuration.ServiceName))
                throw new ArgumentNullException(nameof(configuration.ServiceName));

            OpenTelemetryServicesExtensions.AddOpenTelemetry(services)
                .ConfigureResource(resource => resource
                    .AddService(configuration.ServiceName)
                    .AddAttributes([
                        new KeyValuePair<string, object>("service.domain", configuration.ServiceDomain)
                    ]))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter());

            return services;
        }
    }

    public class OpenTelemetryConfiguration
    {
        public string ServiceName { get; set; }
        public string ServiceDomain { get; set; }
    }
}
