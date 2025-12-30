using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Domain;
using SampleApp.Infrastructure;
using Xunit;

namespace SampleAppProject;

public class UserTests(SampleAppFixture fixture) : SampleAppTestBase(fixture)
{
    [Fact]
    public async Task GetUsers_ReturnsManuallyAddedUsers()
    {
        // Arrange
        var dbContext = Services.GetRequiredService<AppDbContext>();
        dbContext.Users.Add(new User { Name = "Tonny Donny", Email = "tonny@example.com"});
        await dbContext.SaveChangesAsync(CancellationToken);

        // Act
        var url = new Uri("/users", UriKind.Relative);
        var response = await Client.GetAsync(url, CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>(CancellationToken);

        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.Name == "Alice Smith");
        Assert.Contains(users, u => u.Name == "Bob Jones");
        Assert.Contains(users, u => u.Name == "Charlie Day");
        Assert.Contains(users, u => u.Name == "Tonny Donny");
    }

    [Fact]
    public async Task GetUsers_ReturnsSeededData()
    {
        // Act
        var url = new Uri("/users", UriKind.Relative);
        var response = await Client.GetAsync(url, CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>(CancellationToken);

        Assert.NotNull(users);
        Assert.NotEmpty(users);
        Assert.Contains(users, u => u.Name == "Alice Smith");
        Assert.Contains(users, u => u.Name == "Bob Jones");
        Assert.Contains(users, u => u.Name == "Charlie Day");
    }

    [Fact]
    public async Task CreateUser_AddsNewUserToDatabase()
    {
        // Arrange
        var newUserQuery = new Dictionary<string, string?>
        {
            { "name", "New User" },
            { "email", "newuser@example.com" }
        };
        var queryString = QueryString.Create(newUserQuery);

        // Act
        var url = new Uri($"/users{queryString}", UriKind.Relative);
        var response = await Client.PostAsync(url, null, CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdUser = await response.Content.ReadFromJsonAsync<User>(CancellationToken);

        Assert.NotNull(createdUser);
        Assert.Equal("New User", createdUser.Name);
        Assert.Equal("newuser@example.com", createdUser.Email);
        Assert.True(createdUser.Id > 0);

        // Verify via Get
        var getAllResponse = await Client.GetFromJsonAsync<List<User>>("/users", CancellationToken);
        Assert.NotNull(getAllResponse);
        Assert.Contains(getAllResponse, u => u.Email == "newuser@example.com");
    }
}


