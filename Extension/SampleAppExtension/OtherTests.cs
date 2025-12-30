using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Domain;
using SampleApp.Infrastructure;
using Xunit;

namespace SampleAppExtension;

public class OtherTests: SampleAppTestBase
{
    [Fact]
    public async Task AddUsers()
    {
        // Arrange
        using var scope = RootServiceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Users.Add(new User { Name = "Tom Dom", Email = "tom@example.com"});
        await dbContext.SaveChangesAsync(CancellationToken);

        // Assert
        var users = await dbContext.Users.ToListAsync(CancellationToken);

        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.Name == "Alice Smith");
        Assert.Contains(users, u => u.Name == "Bob Jones");
        Assert.Contains(users, u => u.Name == "Charlie Day");
        Assert.Contains(users, u => u.Name == "Tom Dom");
    }

    [Fact]
    public async Task UsingDbContextFromScope_WillReturnCachedData()
    {
        // Arrange
        using var rootScope = RootServiceProvider.CreateScope();

        var dbContext = rootScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userToAdd = new User { Name = "Danny Duck", Email = "danny@example.com"};
        dbContext.Users.Add(userToAdd);
        await dbContext.SaveChangesAsync(CancellationToken);

        // Act
        await Client.PutAsync($"/users/{userToAdd.Id}?name=Danny Duck 2&email={userToAdd.Email}", null, CancellationToken);

        // Assert
        var users = await dbContext.Users.ToListAsync(CancellationToken);
        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.Name == "Danny Duck");

        using (var scope = CreateServiceScope())
        {
            var anotherDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await anotherDbContext.Users.SingleOrDefaultAsync(u => u.Name == "Danny Duck", CancellationToken);
            Assert.Null(user);
        }

        using (var scope = CreateServiceScope())
        {
            var anotherDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await anotherDbContext.Users.SingleOrDefaultAsync(u => u.Name == "Danny Duck 2", CancellationToken);
            Assert.NotNull(user);
        }
    }
}
