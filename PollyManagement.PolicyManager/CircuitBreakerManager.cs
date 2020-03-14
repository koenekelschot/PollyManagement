using Polly.CircuitBreaker;
using Polly.Registry;
using System;
using System.Collections.Generic;

namespace PollyManagement.PolicyManager
{
    public class CircuitBreakerManager
    {
        internal readonly PolicyRegistry Registry;

        public CircuitBreakerManager()
        {
            Registry = new PolicyRegistry();
        }

        public TPolicy GetOrAdd<TPolicy>(string key, TPolicy policy) where TPolicy : ICircuitBreakerPolicy
        {
            return Registry.GetOrAdd(key, policy);
        }

        //public CircuitBreakerPolicy GetOrAdd<ICircuitBreakerPolicy>(string key, Func<string, CircuitBreakerPolicy> policyFactory)
        //{
        //    return Registry.GetOrAdd(key, policyFactory);
        //}

        //public CircuitBreakerPolicy<TResult> GetOrAdd<ICircuitBreakerPolicy>(string key, CircuitBreakerPolicy<TResult> policy)
        //{
        //    return (CircuitBreakerPolicy<TResult>)_policyRegistry.GetOrAdd(key, policy);
        //}

        public IEnumerable<string> GetKeys()
        {
            foreach (var policy in Registry)
            {
                yield return policy.Key;
            }
        }

        public CircuitState GetCircuitState(string key)
        {
            ThrowOnNotRegistered(key, out ICircuitBreakerPolicy policy);

            return policy.CircuitState;
        }

        public Exception GetLastException(string key)
        {
            ThrowOnNotRegistered(key, out ICircuitBreakerPolicy policy);

            return policy.LastException;
        }

        public bool TryIsolate(string key)
        {
            ThrowOnNotRegistered(key, out ICircuitBreakerPolicy policy);

            policy.Isolate();
            return policy.CircuitState == CircuitState.Isolated;
        }

        public bool TryReset(string key)
        {
            ThrowOnNotRegistered(key, out ICircuitBreakerPolicy policy);

            policy.Reset();
            return policy.CircuitState == CircuitState.Closed;
        }

        private void ThrowOnNotRegistered(string key, out ICircuitBreakerPolicy policy)
        {
            if (!Registry.TryGet(key, out policy))
            {
                throw new ArgumentException($"No circuitbreaker registered with this key {key}", nameof(key));
            }
        }
    }
}
