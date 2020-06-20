# PollyManagement

Pollymanagement is a simple library to manage your [Polly](https://github.com/App-vNext/Polly) CircuitBreaker policies and to provide insight in their state.

## Basic usage
`PollyManagement.PolicyManager.CircuitBreakerManager` can be used to register all of the CircuitBreaker policies that you use within your application. First, register the `CircuitBreakerManager` in your DI container. Once added to the container, you can add policies to the CircuitBreakerManager by calling `TryAdd<TPolicy>(string key, TPolicy policy)`: `key` is a unique identifier for the `policy` which can be used to retrieve the policy in places where you need it. To retrieve a policy, call `TryGet(string key)`.

`PollyManagement.ServiceCollection.CircuitBreakerManagerExtensions` contains some helper methods to register the CircuitBreakerManager and policies for usage with `Microsoft.Extensions.DependencyInjection`.

## Advanced usage
For more information about how CircuitBreakers work, see the [Polly Wiki on CircuitBreakers](https://github.com/App-vNext/Polly/wiki/Circuit-Breaker#how-the-polly-circuitbreaker-works).

### Get state of a CircuitBreaker
When you want to know whether an external API is reachable, you can use the CircuitBreakerManager to get some details on the state of the CircuitBreaker.
First, add your policy to the CircuitBreakerManager as described in the Basic Usage section.
To get the status, for example in a healthcheck, you can use `GetCircuitState(string key)` where `key` is the identifier of the policy. 
You can also get the last exception handled by the policy. Use `GetLastException(string key)` to do so.

### Manually "reset" a CircuitBreaker
Sometimes you don't want to wait on a CircuitBreaker to reset. For example, when you are certain that an external API is reachable again. In those cases, you can use the CircuitBreakerManager to manually set the state to [Closed](https://github.com/App-vNext/Polly/wiki/Circuit-Breaker#closed). To do so, use `TryReset(string key)`.

## Examples
`PollyManagement.Webapplication` contains some examples of how CircuitBreakers should (and should not) be used:
- `WrongController` explains the way you shoudn't use CircuitBreakers;
- `BetterController` uses the same approach as `WrongController` but with the use of the CircuitBreakerManager. Making it less prone to subtle errors.
- `IdealController` shows the ideal way to register your CircuitBreakers (i.e. not within a controller).
- `ManagementController` shows how the CircuitBreakerManager can be used to show the state of your CircuitBreakers and explains why `BetterController` isn't ideal.
- `Startup.ConfigureServices(IServiceCollection services)` shows how a CircuitBreaker policy can be registered at the start of an application using the `PollyManagement.ServiceCollection.CircuitBreakerManagerExtensions`.