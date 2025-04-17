using Moq;
using SmartLibrary.Domain.Entities;
using SmartLibrary.Application.Interfaces;
using SmartLibrary.Application.Services;

namespace UnitTests.Application.Services;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private UserService _userService;
    
    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Test]
    public async Task RegisterUserAsync_ShouldCallAddUserAsync()
    {
        // Arrange
        var firstname = "John";
        var lastname = "Doe";
        var email = "testuser@example.com";
        var password = "password123";
        var membershipTypeId = 1;

        // Act
        await _userService.RegisterUserAsync(firstname, lastname, email, password, membershipTypeId);

        // Assert
        _mockUserRepository.Verify(repo => repo.AddUserAsync(It.Is<User>(u =>
            u.FirstName == firstname &&
            u.LastName == lastname &&
            u.Email == email &&
            BCrypt.Net.BCrypt.Verify(password, u.PasswordHash, false, BCrypt.Net.HashType.SHA384) &&
            u.IsActive == true &&
            u.MembershipTypeID == membershipTypeId &&
            u.RenewalDate > DateTime.UtcNow
        )), Times.Once);
    }
    
    [Test]
    public void RegisterUserAsync_WithEmptyEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var firstname = "John";
        var lastname = "Doe";
        var email = "";  // Lege email
        var password = "password123";

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.RegisterUserAsync(firstname, lastname, email, password));
        
        Assert.That(exception.Message, Contains.Substring("email"));
        _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Test]
    public void RegisterUserAsync_WithEmptyFirstName_ShouldThrowArgumentException()
    {
        // Arrange
        string firstname = "";
        var lastname = "Doe";
        var email = "test@example.com";
        var password = "password123";

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.RegisterUserAsync(firstname, lastname, email, password));
        
        Assert.That(exception.Message, Contains.Substring("firstname"));
        _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Test]
    public void RegisterUserAsync_WithShortPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var firstname = "John";
        var lastname = "Doe";
        var email = "test@example.com";
        var password = "short";  // Te kort wachtwoord

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.RegisterUserAsync(firstname, lastname, email, password));
        
        Assert.That(exception.Message, Contains.Substring("password"));
        _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Test]
    public void RegisterUserAsync_WithInvalidEmailFormat_ShouldThrowArgumentException()
    {
        // Arrange
        var firstname = "John";
        var lastname = "Doe";
        var email = "invalid-email";  // Geen geldig e-mailformaat
        var password = "password123";

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.RegisterUserAsync(firstname, lastname, email, password));
        
        Assert.That(exception.Message, Contains.Substring("email"));
        _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Test]
    public void RegisterUserAsync_WithInvalidMembershipTypeId_ShouldThrowArgumentException()
    {
        // Arrange
        var firstname = "John";
        var lastname = "Doe";
        var email = "test@example.com";
        var password = "password123";
        var membershipTypeId = 0;  // Ongeldige membership type id

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.RegisterUserAsync(firstname, lastname, email, password, membershipTypeId));
        
        Assert.That(exception.Message, Contains.Substring("lidmaatschapstype"));
        _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Never);
    }
    
    [Test]
    public async Task DeactivateUserAsync_ShouldUpdateUser()
    {
        // Arrange
        var userId = 1;
        var users = new List<User> { new User {        
            Id = userId, 
            IsActive = true,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hashedpassword"
            
        }};
        _mockUserRepository.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

        // Act
        await _userService.DeactivateUserAsync(userId);

        // Assert
        _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.Is<User>(u => 
            u.Id == userId && 
            u.IsActive == false)), Times.Once);
    }
    
    [Test]
    public void DeactivateUserAsync_UserNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 999; // Niet-bestaande user
        var users = new List<User> { 
            new User { 
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = "hashedpassword"
            } 
        };
        _mockUserRepository.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.DeactivateUserAsync(userId));
        
        Assert.That(exception.Message, Contains.Substring("niet gevonden"));
    }
    
    [Test]
    public async Task RenewMembershipAsync_ShouldUpdateUser()
    {
        // Arrange
        var userId = 1;
        var users = new List<User> { 
            new User { 
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = "hashedpassword"
            } 
        };
        var newRenewalDate = DateTime.UtcNow.AddYears(2);
        _mockUserRepository.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

        // Act
        await _userService.RenewMembershipAsync(userId, newRenewalDate);

        // Assert
        _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.Is<User>(u => 
            u.Id == userId && 
            u.RenewalDate == newRenewalDate &&
            u.IsActive == true)), Times.Once);
    }
    
    [Test]
    public void RenewMembershipAsync_PastDate_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        var users = new List<User> { 
            new User { 
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = "hashedpassword"
            } 
        };
        var pastDate = DateTime.UtcNow.AddDays(-1);
        _mockUserRepository.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.RenewMembershipAsync(userId, pastDate));
        
        Assert.That(exception.Message, Contains.Substring("toekomst"));
    }
    
    [Test]
    public async Task ChangeMembershipTypeAsync_ShouldUpdateUser()
    {
        // Arrange
        var userId = 1;
        var users = new List<User> { 
            new User { 
                Id = userId, 
                MembershipTypeID = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = "hashedpassword"
            } 
        };
        var newMembershipTypeId = 2;
        _mockUserRepository.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

        // Act
        await _userService.ChangeMembershipTypeAsync(userId, newMembershipTypeId);

        // Assert
        _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.Is<User>(u => 
            u.Id == userId && 
            u.MembershipTypeID == newMembershipTypeId)), Times.Once);
    }
    
    [Test]
    public void ChangeMembershipTypeAsync_InvalidType_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = 1;
        var users = new List<User> { 
            new User { 
                Id = userId, 
                MembershipTypeID = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = "hashedpassword"
            } 
        };
        var invalidTypeId = 0;
        _mockUserRepository.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _userService.ChangeMembershipTypeAsync(userId, invalidTypeId));
        
        Assert.That(exception.Message, Contains.Substring("ongeldig"));
    }
}