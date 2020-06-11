using Microsoft.Extensions.DependencyInjection;
using Polly.CircuitBreaker;
using PollyManagement.PolicyManager;
using System.Collections.Generic;

namespace PollyManagement.Webapplication.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPollyPolicy<TPolicy>(this IServiceCollection services, string policyName, TPolicy policy) where TPolicy : ICircuitBreakerPolicy
        {
            var manager = GetOrAddManager(services);
            manager.TryAdd(policyName, policy);

            return services;
        }

        public static IServiceCollection AddPollyPolicies<TPolicy>(this IServiceCollection services, IDictionary<string, TPolicy> policies) where TPolicy : ICircuitBreakerPolicy
        {
            var manager = GetOrAddManager(services);
            foreach (var policy in policies)
            {
                manager.TryAdd(policy.Key, policy.Value);
            }

            return services;
        }

        private static CircuitBreakerManager GetOrAddManager(IServiceCollection services)
        {
            CircuitBreakerManager manager;

            using (var provider = services.BuildServiceProvider())
            {
                manager = provider.GetService<CircuitBreakerManager>();
                if (manager == null)
                {
                    manager = new CircuitBreakerManager();
                    services.AddSingleton(manager);
                }
            }

            return manager;
        }
    }
}
