using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SampleApp.Domain;
using SampleApp.Services;
using Xunit;

namespace SampleApp.Tests;

public class UserTests(SampleAppFactory factory) : SampleAppTestBase(factory)
{
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


