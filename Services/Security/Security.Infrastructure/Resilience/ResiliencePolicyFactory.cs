using System;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace Security.Infrastructure.Resilience
{
    /// <summary>
    /// Builds the retry + circuit breaker pipeline shared by the external clients
    /// (Kafka, Elasticsearch). Retry is the outer policy and circuit breaker the inner one,
    /// per Polly's own guidance: once the breaker opens, retries fail fast with
    /// BrokenCircuitException instead of hammering a dependency that's already known to be down.
    /// </summary>
    public static class ResiliencePolicyFactory
    {
        public static IAsyncPolicy CreateDefault(ILogger logger, string dependencyName)
        {
            var retry = Policy
                .Handle<Exception>(ex => ex is not BrokenCircuitException)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1)),
                    onRetry: (exception, delay, attempt, _) =>
                        logger.LogWarning(exception,
                            "{Dependency} call failed (attempt {Attempt}), retrying in {Delay}ms",
                            dependencyName, attempt, delay.TotalMilliseconds));

            var circuitBreaker = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, breakDuration) =>
                        logger.LogError(exception,
                            "{Dependency} circuit breaker opened for {BreakDuration}s after repeated failures",
                            dependencyName, breakDuration.TotalSeconds),
                    onReset: () => logger.LogInformation("{Dependency} circuit breaker closed", dependencyName),
                    onHalfOpen: () => logger.LogInformation("{Dependency} circuit breaker half-open, testing dependency", dependencyName));

            return Policy.WrapAsync(retry, circuitBreaker);
        }
    }
}
