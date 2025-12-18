using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace XUnitFramework.Project;

public abstract class BaseIntegrationTest<TProgram> : IAsyncLifetime
    where TProgram : class
{
    private readonly IServiceScope _scope;

    protected IntegrationTestFixture<TProgram> Fixture { get; }
    protected HttpClient Client { get; }
    protected IServiceProvider Services => _scope.ServiceProvider;

    protected BaseIntegrationTest(IntegrationTestFixture<TProgram> fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        Fixture = fixture;
        Client = fixture.CreateClient();
        _scope = fixture.Services.CreateScope();
    }

    protected IServiceScope CreateScope() => Fixture.Services.CreateScope();

    public virtual ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask DisposeAsync()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
