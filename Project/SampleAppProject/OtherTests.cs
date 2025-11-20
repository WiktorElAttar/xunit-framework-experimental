using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Domain;
using SampleApp.Infrastructure;
using Xunit;

namespace SampleAppProject;

public class OtherTests(SampleAppFixture fixture) : SampleAppTestBase(fixture)
{
    [Fact]
    public async Task AddUsers()
    {
        // Arrange
        var dbContext = Services.GetRequiredService<AppDbContext>();
        dbContext.Users.Add(new User { Name = "Danny Duck", Email = "danny@example.com"});
        await dbContext.SaveChangesAsync();

        // Assert
        var users = await dbContext.Users.ToListAsync();

        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.Name == "Alice Smith");
        Assert.Contains(users, u => u.Name == "Bob Jones");
        Assert.Contains(users, u => u.Name == "Charlie Day");
        Assert.Contains(users, u => u.Name == "Danny Duck");
    }

    [Fact]
    public async Task UsingDbContextFromScope_WillReturnCachedData()
    {
        // Arrange
        var dbContext = Services.GetRequiredService<AppDbContext>();
        var userToAdd = new User { Name = "Danny Duck", Email = "danny@example.com"};
        dbContext.Users.Add(userToAdd);
        await dbContext.SaveChangesAsync();

        // Act
        await Client.PutAsync($"/users/{userToAdd.Id}?name={"Danny Duck 2"}&email={userToAdd.Email}", null);

        // Assert
        var users = await dbContext.Users.ToListAsync();
        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.Name == "Danny Duck");

        using (var scope = Services.CreateScope())
        {
            var anotherDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await anotherDbContext.Users.SingleOrDefaultAsync(u => u.Name == "Danny Duck");
            Assert.Null(user);
        }

        using (var scope = Services.CreateScope())
        {
            var anotherDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await anotherDbContext.Users.SingleOrDefaultAsync(u => u.Name == "Danny Duck 2");
            Assert.NotNull(user);
        }
    }
}
