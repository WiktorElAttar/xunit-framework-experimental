using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SampleApp.Domain;
using SampleApp.Infrastructure;
using SampleApp.Services;
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
        var response = await Client.GetAsync("/users", CancellationToken);

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
        var response = await Client.GetAsync("/users", CancellationToken);

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
        // Using PostAsync with query string since the endpoint binds to query parameters
        var response = await Client.PostAsync($"/users{queryString}", null, CancellationToken);

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


