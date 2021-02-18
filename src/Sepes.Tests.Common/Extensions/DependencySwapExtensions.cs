using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Sepes.Tests.Common.Extensions
{
    public static class DependencySwapExtensions
    {     
        public static void SwapTransient<TService, TImplementation>(this IServiceCollection services)
            where TImplementation : class, TService
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient).ToList();

                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }

            services.AddTransient(typeof(TService), typeof(TImplementation));
        }

        public static void SwapTransientWithScoped<TService, TImplementation>(this IServiceCollection services)
           where TImplementation : class, TService
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient).ToList();

                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }

            services.AddScoped(typeof(TService), typeof(TImplementation));
        }

        public static void SwapTransientWithSingleton<TService, TImplementation>(this IServiceCollection services)
       where TImplementation : class, TService
        {
            if (services.Any(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient))
            {
                var serviceDescriptors = services.Where(x => x.ServiceType == typeof(TService) && x.Lifetime == ServiceLifetime.Transient).ToList();

                foreach (var serviceDescriptor in serviceDescriptors)
                {
                    services.Remove(serviceDescriptor);
                }
            }

            services.AddSingleton(typeof(TService), typeof(TImplementation));
        }
    }
}
