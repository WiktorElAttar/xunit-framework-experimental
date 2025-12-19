using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace XUnitFramework.Extension;

public abstract class BaseIntegrationTest<TProgram, TFixture> : IAsyncLifetime
    where TProgram : class
    where TFixture : IntegrationTestFixture<TProgram>
{
    private IServiceScope _scope = null!;

    protected TFixture Fixture { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected IServiceProvider Services => _scope.ServiceProvider;
    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    protected BaseIntegrationTest()
    {
    }

    protected IServiceScope CreateScope() => Fixture.Services.CreateScope();

    public virtual async ValueTask InitializeAsync()
    {
        Fixture = await TestContext.Current.GetFixture<TFixture>()
            ?? throw new InvalidOperationException($"Fixture of type {nameof(TFixture)} was not found in the test context.");
        Client = Fixture.CreateClient();
        _scope = Fixture.Services.CreateScope();
    }

    public virtual ValueTask DisposeAsync()
    {
        _scope?.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
