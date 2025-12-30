using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace XUnitFramework.Extension;

public abstract class IntegrationTestBase<TProgram, TFixture> : IAsyncLifetime
    where TProgram : class
    where TFixture : IntegrationTestFixtureBase<TProgram>
{
    private TFixture Fixture { get; set; } = null!;

    protected HttpClient Client { get; private set; } = null!;
    protected IServiceProvider RootServiceProvider => Fixture.Services;
    protected CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    protected IntegrationTestBase() { }

    protected IServiceScope CreateServiceScope() => Fixture.Services.CreateScope();

    public virtual async ValueTask InitializeAsync()
    {
        Fixture = await TestContext.Current.GetFixture<TFixture>()
            ?? throw new InvalidOperationException($"Fixture of type {nameof(TFixture)} was not found in the test context.");
        Client = Fixture.CreateClient();
    }

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
