using SmartLibrary.Application.Interfaces;
using SmartLibrary.Domain.Entities;

namespace SmartLibrary.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task RegisterUserAsync(string firstname, string lastname, string email, string password, int membershipTypeId = 1)
        {
            // Validatie
            if (string.IsNullOrWhiteSpace(firstname))
                throw new ArgumentException("Voornaam mag niet leeg zijn.", nameof(firstname));
        
            if (string.IsNullOrWhiteSpace(lastname))
                throw new ArgumentException("Achternaam mag niet leeg zijn.", nameof(lastname));
        
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail mag niet leeg zijn.", nameof(email));
        
            if (!IsValidEmail(email))
                throw new ArgumentException("E-mail heeft geen geldig formaat.", nameof(email));
        
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                throw new ArgumentException("Wachtwoord moet minimaal 8 tekens bevatten.", nameof(password));

            if (membershipTypeId <= 0)
                throw new ArgumentException("Lidmaatschapstype is ongeldig.", nameof(membershipTypeId));

            var now = DateTime.UtcNow;
            
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsActive = true,
                RegistrationDate = now,
                RenewalDate = now.AddYears(1), // Standaard verlenging na 1 jaar
                MembershipTypeID = membershipTypeId
            };

            await _userRepository.AddUserAsync(user);
        }
        
        // Voeg eventueel extra methoden toe om de gebruiker te beheren
        public async Task DeactivateUserAsync(int userId)
        {
            var users = await _userRepository.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.Id == userId);
            
            if (user == null)
                throw new ArgumentException("Gebruiker niet gevonden.", nameof(userId));
                
            user.IsActive = false;
            await _userRepository.UpdateUserAsync(user);
        }
        
        public async Task RenewMembershipAsync(int userId, DateTime newRenewalDate)
        {
            var users = await _userRepository.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.Id == userId);
            
            if (user == null)
                throw new ArgumentException("Gebruiker niet gevonden.", nameof(userId));
                
            if (newRenewalDate <= DateTime.UtcNow)
                throw new ArgumentException("Verlengingsdatum moet in de toekomst liggen.", nameof(newRenewalDate));
                
            user.RenewalDate = newRenewalDate;
            user.IsActive = true; // Activeer de gebruiker weer als deze inactief was
            
            await _userRepository.UpdateUserAsync(user);
        }
        
        public async Task ChangeMembershipTypeAsync(int userId, int newMembershipTypeId)
        {
            var users = await _userRepository.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.Id == userId);
            
            if (user == null)
                throw new ArgumentException("Gebruiker niet gevonden.", nameof(userId));
                
            if (newMembershipTypeId <= 0)
                throw new ArgumentException("Lidmaatschapstype is ongeldig.", nameof(newMembershipTypeId));
                
            user.MembershipTypeID = newMembershipTypeId;
            
            await _userRepository.UpdateUserAsync(user);
        }
        
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}