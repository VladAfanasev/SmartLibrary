using SmartLibrary.Domain.Interfaces;

namespace SmartLibrary.Application.Interfaces;

public interface IUserService
{
    Task<List<IUser>> GetAllUsersAsync();
    Task<IUser> GetUserByIdAsync(int id);
    Task<IUser> GetUserByEmailAsync(string email);
    Task<bool> AddUserAsync(IUser user);
    Task<bool> UpdateUserAsync(IUser user);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> DeactivateUserAsync(int userId);
    Task<bool> RenewMembershipAsync(int userId, DateTime newRenewalDate);
    Task<bool> ChangeMembershipTypeAsync(int userId, int newMembershipTypeId);
}