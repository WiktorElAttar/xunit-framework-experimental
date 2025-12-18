using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Domain;
using SampleApp.Infrastructure;
using Xunit;

namespace SampleAppExtension;

public class UserTests : SampleAppTestBase
{
    [Fact]
    public async Task GetUsers_ReturnsManuallyAddedUsers()
    {
        // Arrange
        var dbContext = Services.GetRequiredService<AppDbContext>();
        dbContext.Users.Add(new User { Name = "Tonny Donny", Email = "tonny@example.com"});
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>();

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
        var response = await Client.GetAsync("/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<User>>();

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
        var response = await Client.PostAsync($"/users{queryString}", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdUser = await response.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(createdUser);
        Assert.Equal("New User", createdUser.Name);
        Assert.Equal("newuser@example.com", createdUser.Email);
        Assert.True(createdUser.Id > 0);

        // Verify via Get
        var getAllResponse = await Client.GetFromJsonAsync<List<User>>("/users");
        Assert.NotNull(getAllResponse);
        Assert.Contains(getAllResponse, u => u.Email == "newuser@example.com");
    }
}


