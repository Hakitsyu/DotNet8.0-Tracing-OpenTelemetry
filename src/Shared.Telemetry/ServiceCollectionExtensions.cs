using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Telemetry
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTelemetryCore(this IServiceCollection services, Action<TelemetryConfiguration>? configure = null)
        {
            TelemetryConfiguration configuration = new();
            configure?.Invoke(configuration);

            if (string.IsNullOrWhiteSpace(configuration.ServiceName))
                throw new ArgumentNullException(nameof(configuration.ServiceName));

            services.AddSingleton(new ActivitySource(configuration.ServiceName));

            return services;
        }
    }

    public class TelemetryConfiguration
    {
        public string ServiceName { get; set; }
    }
}
