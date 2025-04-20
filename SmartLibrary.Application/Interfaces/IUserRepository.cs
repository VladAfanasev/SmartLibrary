using SmartLibrary.Domain.Interfaces;

namespace SmartLibrary.Application.Interfaces;

public interface IUserRepository
{
    Task<List<IUser>> GetAllUsersAsync();
    Task<IUser> GetUserByIdAsync(int userId);
    Task<IUser> GetUserByEmailAsync(string email);
    Task AddUserAsync(IUser user);
    Task UpdateUserAsync(IUser user);
    Task DeleteUserAsync(int userId);

}