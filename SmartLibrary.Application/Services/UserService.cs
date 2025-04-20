using Microsoft.Extensions.Logging;
using SmartLibrary.Application.Interfaces;
using SmartLibrary.Domain.Interfaces;

namespace SmartLibrary.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<IUser>> GetAllUsersAsync()
        {
            _logger.LogInformation("Getting all users");
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<IUser> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Getting user by id: {Id}", id);
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<IUser> GetUserByEmailAsync(string email)
        {
            _logger.LogInformation("Getting user by email: {Email}", email);
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<bool> AddUserAsync(IUser user)
        {
            // Validate user properties
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");

            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new ArgumentException("Voornaam mag niet leeg zijn.", "firstname");

            if (string.IsNullOrWhiteSpace(user.LastName))
                throw new ArgumentException("Achternaam mag niet leeg zijn.", "lastname");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("E-mailadres mag niet leeg zijn.", "email");

            if (!IsValidEmail(user.Email))
                throw new ArgumentException("E-mailadres heeft een ongeldig formaat.", "email");

            // In your test, PasswordHash is used to pass the unhashed password for validation
            if (string.IsNullOrWhiteSpace(user.PasswordHash) || user.PasswordHash.Length < 8)
                throw new ArgumentException("Wachtwoord moet minimaal 8 tekens bevatten.", "password");

            if (user.MembershipTypeID <= 0)
                throw new ArgumentException("Lidmaatschapstype moet een geldige waarde hebben.", "membershipTypeId");

            try
            {
                _logger.LogInformation("Adding new user: {Email}", user.Email);
                await _userRepository.AddUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {Email}", user.Email);
                return false;
            }
        }
        public async Task<bool> UpdateUserAsync(IUser user)
        {
            try
            {
                _logger.LogInformation("Updating user: {Id}, {Email}", user.Id, user.Email);
                await _userRepository.UpdateUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", user.Id);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Deleting user by id: {Id}", userId);
                await _userRepository.DeleteUserAsync(userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", userId);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(IUser user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to delete null user");
                return false;
            }

            return await DeleteUserAsync(user.Id);
        }
        
        public async Task<bool> DeactivateUserAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Deactivating user: {Id}", userId);
                
                // Get all users and find the one to deactivate
                var users = await _userRepository.GetAllUsersAsync();
                var user = users.FirstOrDefault(u => u.Id == userId);
                
                if (user == null)
                    throw new ArgumentException($"Gebruiker met ID {userId} niet gevonden.");
                
                // Deactivate the user
                user.IsActive = false;
                
                // Update in the database
                await _userRepository.UpdateUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {Id}", userId);
                throw; // Rethrow to let test catch the exception
            }
        }
        
        
        public async Task<bool> RenewMembershipAsync(int userId, DateTime newRenewalDate)
        {
            // Validate the renewal date
            if (newRenewalDate <= DateTime.UtcNow)
                throw new ArgumentException("Verlengingsdatum moet in de toekomst liggen.", nameof(newRenewalDate));
                
            try
            {
                _logger.LogInformation("Renewing membership for user: {Id}", userId);
                
                // Get all users and find the one to update
                var users = await _userRepository.GetAllUsersAsync();
                var user = users.FirstOrDefault(u => u.Id == userId);
                
                if (user == null)
                    throw new ArgumentException($"Gebruiker met ID {userId} niet gevonden.");
                
                // Update renewal date and ensure active
                user.RenewalDate = newRenewalDate;
                user.IsActive = true;
                
                // Update in the database
                await _userRepository.UpdateUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing membership for user: {Id}", userId);
                throw; // Rethrow to let test catch the exception
            }
        }
        
        public async Task<bool> ChangeMembershipTypeAsync(int userId, int newMembershipTypeId)
        {
            // Validate membership type
            if (newMembershipTypeId <= 0)
                throw new ArgumentException("Lidmaatschapstype is ongeldig.", nameof(newMembershipTypeId));
                
            try
            {
                _logger.LogInformation("Changing membership type for user: {Id} to {Type}", userId, newMembershipTypeId);
                
                // Get all users and find the one to update
                var users = await _userRepository.GetAllUsersAsync();
                var user = users.FirstOrDefault(u => u.Id == userId);
                
                if (user == null)
                    throw new ArgumentException($"Gebruiker met ID {userId} niet gevonden.");
                
                // Update membership type
                user.MembershipTypeID = newMembershipTypeId;
                
                // Update in the database
                await _userRepository.UpdateUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing membership type for user: {Id}", userId);
                throw; // Rethrow to let test catch the exception
            }
        }
        
        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
    
            try
            {
                // Simple regex pattern for basic email validation
                return System.Text.RegularExpressions.Regex.IsMatch(email, 
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}