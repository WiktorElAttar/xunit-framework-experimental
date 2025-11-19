using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SampleApp.Domain;
using SampleApp.Services;
using Xunit;

namespace SampleApp.Tests;

public class OverrideUserTests(SampleAppFactory factory) : SampleAppTestBase(factory)
{
    [Fact]
    public async Task GetUsers_ReturnsSeededData()
    {
        // Act
        var response = await Client.GetAsync("/users");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
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
        var response = await Client.PostAsync($"/users{queryString}", null);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.RemoveAll<IUserService>();
        services.AddScoped<IUserService, MockUserService>();
    }

    private class MockUserService : IUserService
    {
        public Task<List<User>> GetAllUsersAsync() => throw new NotImplementedException();

        public Task<User> CreateUserAsync(string name, string email) => throw new NotImplementedException();
    }
}


