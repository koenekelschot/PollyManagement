using Polly.CircuitBreaker;
using Polly.Registry;
using System;
using System.Collections.Generic;

namespace PollyManagement.PolicyManager
{
    public class CircuitBreakerManager
    {
        internal readonly PolicyRegistry Registry;
        private readonly object _lock = new object();

        public CircuitBreakerManager()
        {
            Registry = new PolicyRegistry();
        }

        public TPolicy GetOrAdd<TPolicy>(string key, TPolicy policy) where TPolicy : ICircuitBreakerPolicy
        {
            //Polly 7.2.0 does implement GetOrAdd
            //return Registry.GetOrAdd(key, policy);
            lock (_lock)
            {
                if (!Registry.TryGet(key, out TPolicy registeredPolicy))
                {
                    Registry.Add(key, policy);
                    registeredPolicy = policy;
                }

                return registeredPolicy;
            }
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

        public IDictionary<string, CircuitState> GetCircuitStates()
        {
            var dict = new Dictionary<string, CircuitState>(Registry.Count);

            foreach (var policy in Registry)
            {
                dict.Add(policy.Key, Registry.Get<ICircuitBreakerPolicy>(policy.Key).CircuitState);
            }

            return dict;
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
