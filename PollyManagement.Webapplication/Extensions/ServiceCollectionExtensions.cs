using Microsoft.Extensions.DependencyInjection;
using Polly.CircuitBreaker;
using PollyManagement.PolicyManager;
using PollyManagement.PolicyManager.Implementations;
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

        private static ICircuitBreakerManager GetOrAddManager(IServiceCollection services)
        {
            ICircuitBreakerManager manager;

            using (var provider = services.BuildServiceProvider())
            {
                manager = provider.GetService<ICircuitBreakerManager>();
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
