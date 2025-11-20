using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SampleApp.Infrastructure;
using Testcontainers.PostgreSql;
using XUnitFramework.Project;

namespace SampleApp.Tests;

public class SampleAppFixture : IntegrationTestFixture<Program>
{
    // Define your containers here
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    public override async ValueTask InitializeAsync()
    {
        // Start containers before tests run
        await _dbContainer.StartAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // Remove the DbContext registration from Program.cs
        services.RemoveAll<DbContextOptions<AppDbContext>>();

        // Register DbContext using the container's connection string
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(_dbContainer.GetConnectionString()));
    }
}
