using OrderSystemManagment.Domain.Entities;

namespace OrderSystemManagment.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByCredentialsAsync(string userName, string password);

    Task<IEnumerable<User>> GetUsersAsync();

    Task<User?> GetUserByIdAsync(int id);

    Task SaveUserAsync(User user);

    Task UpdateUserAsync(User user);

    Task SoftDeleteUserAsync(int id);

    Task<IEnumerable<Role>> GetRolesAsync();

    Task<Role?> GetRoleByIdAsync(int id);

    Task SaveRoleAsync(Role role);
}
