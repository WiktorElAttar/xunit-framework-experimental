using SampleApp.Domain;

namespace SampleApp.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(string name, string email);
    Task UpdateUserAsync(int id, string name, string email);

}
