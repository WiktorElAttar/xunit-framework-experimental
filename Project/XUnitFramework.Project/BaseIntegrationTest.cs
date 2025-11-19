using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace XUnitFramework.Project;

public abstract class BaseIntegrationTest<TProgram> : IAsyncLifetime
    where TProgram : class
{
    protected readonly IntegrationTestFactory<TProgram> Factory;

    private readonly WebApplicationFactory<TProgram> _testSpecificFactory;

    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;

    protected BaseIntegrationTest(IntegrationTestFactory<TProgram> factory)
    {
        Factory = factory;

        // Create a "fork" of the factory for this specific test instance.
        // This allows us to inject test-specific services without affecting other tests sharing the fixture.
        _testSpecificFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                ConfigureServices(services);
            });
        });

        // Initialize Client and Scope using the FORKED factory
        Client = _testSpecificFactory.CreateClient();
        Scope = _testSpecificFactory.Services.CreateScope();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    public virtual ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public virtual async ValueTask DisposeAsync()
    {
        // Clean up the scope at the end of the test
        Scope.Dispose();
        await _testSpecificFactory.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
