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

        public async Task RegisterUserAsync(string firstname, string lastname, string email, string password)
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

            
            var user = new User
            {
                FirstName = firstname,
                LastName = lastname,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);
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