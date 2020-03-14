using NUnit.Framework;
using Polly;
using Polly.CircuitBreaker;
using System;
using System.Linq;

namespace PollyManagement.PolicyManager.Test
{
    internal class CircuitBreakerManagerTest
    {
        private CircuitBreakerManager _sut;
        private CircuitBreakerPolicy _testPolicy;
        private const string TestKey = "test";

        [SetUp]
        public void Setup()
        {
            _sut = new CircuitBreakerManager();
            _sut.Registry.Clear();
            _testPolicy = GetCircuitBreakerPolicy();
        }

        [Test]
        public void GetOrAdd_IfPolicyDoesNotExist_ThenReturnAddedPolicy()
        {
            var policy = _sut.GetOrAdd<CircuitBreakerPolicy>(TestKey, _testPolicy);

            Assert.AreEqual(1, _sut.Registry.Count);
            Assert.IsTrue(_sut.Registry.ContainsKey(TestKey));
            Assert.AreSame(_testPolicy, policy);
        }

        [Test]
        public void GetOrAdd_IfPolicyWithDifferentKeyCasing_ThenReturnAddedPolicy()
        {
            var initialPolicy = GetCircuitBreakerPolicy();
            _sut.Registry.Add(TestKey, initialPolicy);
            var policy = _sut.GetOrAdd<CircuitBreakerPolicy>(TestKey.ToUpper(), _testPolicy);

            Assert.AreEqual(2, _sut.Registry.Count);
            Assert.IsTrue(_sut.Registry.ContainsKey(TestKey));
            Assert.AreSame(_testPolicy, policy);
            Assert.AreNotSame(initialPolicy, policy);
        }

        [Test]
        public void GetOrAdd_IfPolicyDoesExist_ThenReturnExistingPolicy()
        {
            _sut.Registry.Add(TestKey, GetCircuitBreakerPolicy());

            var policy = _sut.GetOrAdd<CircuitBreakerPolicy>(TestKey, _testPolicy);

            Assert.AreEqual(1, _sut.Registry.Count);
            Assert.IsTrue(_sut.Registry.ContainsKey(TestKey));
            Assert.AreNotSame(_testPolicy, policy);
        }

        [Test]
        public void GetKeys_IfDoesNotContainPolicies_ThenReturnEmpty()
        {
            var keys = _sut.GetKeys();

            Assert.IsEmpty(keys);
        }

        [Test]
        public void GetKeys_IfDoesContainPolicies_ThenReturnAllKeys()
        {
            _sut.Registry.Add("a", GetCircuitBreakerPolicy());
            _sut.Registry.Add("b", GetCircuitBreakerPolicy());

            var keys = _sut.GetKeys();

            Assert.IsNotEmpty(keys);
            Assert.IsTrue(keys.Any(key => key == "a"));
            Assert.IsTrue(keys.Any(key => key == "b"));
            Assert.IsFalse(keys.Any(key => key == "c"));
        }

        [Test]
        public void GetCircuitState_IfKeyDoesNotExist_ThenThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.GetCircuitState(TestKey));
        }

        [Test]
        public void GetCircuitState_IfKeyDoesExist_ThenReturnResult()
        {
            var policy = GetCircuitBreakerPolicy();
            _sut.Registry.Add(TestKey, policy);

            var state = _sut.GetCircuitState(TestKey);
            Assert.AreEqual(policy.CircuitState, state);
        }

        [Test]
        public void GetLastException_IfKeyDoesNotExist_ThenThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.GetLastException(TestKey));
        }

        [Test]
        public void GetLastException_IfKeyDoesExist_ThenReturnResult()
        {
            var policy = GetCircuitBreakerPolicy();
            _sut.Registry.Add(TestKey, policy);

            var exception = _sut.GetLastException(TestKey);
            Assert.AreEqual(policy.LastException, exception);
        }

        [Test]
        public void TryIsolate_IfKeyDoesNotExist_ThenThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.TryIsolate(TestKey));
        }

        [Test]
        public void TryIsolate_IfKeyDoesExist_ThenReturnResult()
        {
            var policy = GetCircuitBreakerPolicy();
            _sut.Registry.Add(TestKey, policy);

            var result = _sut.TryIsolate(TestKey);
            Assert.IsTrue(result);
        }

        [Test]
        public void TryReset_IfKeyDoesNotExist_ThenThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.TryReset(TestKey));
        }

        [Test]
        public void TryReset_IfKeyDoesExist_ThenReturnResult()
        {
            var policy = GetCircuitBreakerPolicy();
            _sut.Registry.Add(TestKey, policy);

            var result = _sut.TryReset(TestKey);
            Assert.IsTrue(result);
        }

        private CircuitBreakerPolicy GetCircuitBreakerPolicy()
        {
            return Policy.Handle<Exception>().AdvancedCircuitBreaker(1.0, TimeSpan.FromSeconds(1), 2, TimeSpan.FromMinutes(1));
        }
    }
}