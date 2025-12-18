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

    protected BaseIntegrationTest()
    {
    }

    protected IServiceScope CreateScope() => Fixture.Services.CreateScope();

    public virtual async ValueTask InitializeAsync()
    {
        var fixture = await TestContext.Current.GetFixture<TFixture>();

        if (fixture == null)
        {
            throw new InvalidOperationException($"Fixture of type {typeof(TFixture).Name} was not found in the test context. Ensure the test class is decorated with [Collection] or IClassFixture<>.");
        }

        Fixture = fixture;
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
