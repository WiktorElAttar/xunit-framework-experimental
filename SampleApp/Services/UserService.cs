using Microsoft.EntityFrameworkCore;
using SampleApp.Domain;
using SampleApp.Infrastructure;

namespace SampleApp.Services;

public class UserService(AppDbContext context) : IUserService
{
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User> CreateUserAsync(string name, string email)
    {
        var user = new User { Name = name, Email = email };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateUserAsync(int id, string name, string email)
    {
        var user = await context.Users.FindAsync(id);
        if (user is null)
        {
            return;
        }

        user.Name = name;
        user.Email = email;

        await context.SaveChangesAsync();
    }
}
