using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Question.Application.Services;
using Question.Application.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Question.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITranslatorService, TranslatorService>();
            services.AddHttpClient<ITranslatorService, TranslatorService>(client =>
            {
                client.BaseAddress = new Uri(configuration["TranslatorUrl"]!);
            });

            return services;
        }
    }
}
