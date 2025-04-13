using SmartLibrary.Domain.Entities;
namespace SmartLibrary.Application.Interfaces;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<List<User>> GetUsersAsync();
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
}