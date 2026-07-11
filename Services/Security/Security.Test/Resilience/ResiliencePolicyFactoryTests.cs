using Microsoft.Extensions.Logging.Abstractions;
using Polly.CircuitBreaker;
using Security.Infrastructure.Resilience;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Security.Test.Resilience
{
    public class ResiliencePolicyFactoryTests
    {
        [Fact]
        public async Task CreateDefault_RetriesOnTransientFailureThenSucceeds()
        {
            var policy = ResiliencePolicyFactory.CreateDefault(NullLogger.Instance, "TestDependency");
            var attempts = 0;

            var result = await policy.ExecuteAsync(() =>
            {
                attempts++;
                if (attempts < 3)
                {
                    throw new InvalidOperationException("transient failure");
                }
                return Task.FromResult("ok");
            });

            result.ShouldBe("ok");
            attempts.ShouldBe(3);
        }

        [Fact]
        public async Task CreateDefault_OpensCircuitAfterRepeatedFailures()
        {
            // A fresh policy instance per test: the circuit breaker is stateful, so reusing
            // one across tests would make them order-dependent.
            var policy = ResiliencePolicyFactory.CreateDefault(NullLogger.Instance, "TestDependency");

            // Drive enough failures to trip the breaker (exceptionsAllowedBeforeBreaking: 5 in
            // ResiliencePolicyFactory). Each call below can itself retry up to 3 times, so a
            // handful of calls is more than enough — don't assert the exact exception type here,
            // since whether a given call surfaces the original exception or BrokenCircuitException
            // depends on exactly when mid-call the breaker trips.
            for (var i = 0; i < 3; i++)
            {
                try
                {
                    await policy.ExecuteAsync(() => throw new InvalidOperationException("down"));
                }
                catch
                {
                    // Expected: either the original exception or BrokenCircuitException.
                }
            }

            // Once open, calls fail fast with BrokenCircuitException instead of invoking the action.
            var invoked = false;
            await Should.ThrowAsync<BrokenCircuitException>(() =>
                policy.ExecuteAsync(() =>
                {
                    invoked = true;
                    return Task.CompletedTask;
                }));

            invoked.ShouldBeFalse();
        }
    }
}
