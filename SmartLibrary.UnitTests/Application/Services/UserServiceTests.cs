using Moq;
using SmartLibrary.Domain.Entities;
using SmartLibrary.Domain.Interfaces;
using SmartLibrary.Application.Interfaces;
using SmartLibrary.Application.Services;
using Microsoft.Extensions.Logging;

namespace UnitTests.Application.Services;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private UserService _userService;
    private Mock<ILogger<UserService>> _mockLogger; // Add mock for ILogger

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>(); // Initialize the logger mock
        _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object); // Pass both dependencies
    }

[Test]
public void AddUserAsync_WithEmptyEmail_ShouldThrowArgumentException()
{
    // Arrange
    string firstName = "John";
    string lastName = "Doe";
    string email = "";  // Empty email
    string password = "password123";
    int membershipTypeId = 1;
    bool isActive = true;
    
    var user = new User
    {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = password,  // Not hashed for this test case
        IsActive = isActive,
        RegistrationDate = DateTime.UtcNow,
        RenewalDate = DateTime.UtcNow.AddYears(1),
        MembershipTypeID = membershipTypeId
    };

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.AddUserAsync(user));
    
    Assert.That(exception.Message, Contains.Substring("email"));
    _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<IUser>()), Times.Never);
}

[Test]
public void AddUserAsync_WithEmptyFirstName_ShouldThrowArgumentException()
{
    // Arrange
    string firstName = "";  // Empty first name
    string lastName = "Doe";
    string email = "test@example.com";
    string password = "password123";
    int membershipTypeId = 1;
    bool isActive = true;
    
    var user = new User
    {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = password,
        IsActive = isActive,
        RegistrationDate = DateTime.UtcNow,
        RenewalDate = DateTime.UtcNow.AddYears(1),
        MembershipTypeID = membershipTypeId
    };

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.AddUserAsync(user));
    
    Assert.That(exception.Message, Contains.Substring("firstname"));
    _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<IUser>()), Times.Never);
}

[Test]
public void AddUserAsync_WithShortPassword_ShouldThrowArgumentException()
{
    // Arrange
    string firstName = "John";
    string lastName = "Doe";
    string email = "test@example.com";
    string password = "short";  // Too short password
    int membershipTypeId = 1;
    bool isActive = true;
    
    var user = new User
    {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = password,  // Using unhashed password to test validation
        IsActive = isActive,
        RegistrationDate = DateTime.UtcNow,
        RenewalDate = DateTime.UtcNow.AddYears(1),
        MembershipTypeID = membershipTypeId
    };

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.AddUserAsync(user));
    
    Assert.That(exception.Message, Contains.Substring("password"));
    _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<IUser>()), Times.Never);
}

[Test]
public void AddUserAsync_WithInvalidEmailFormat_ShouldThrowArgumentException()
{
    // Arrange
    string firstName = "John";
    string lastName = "Doe";
    string email = "invalid-email";  // Invalid email format
    string password = "password123";
    int membershipTypeId = 1;
    bool isActive = true;
    
    var user = new User
    {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = password,
        IsActive = isActive,
        RegistrationDate = DateTime.UtcNow,
        RenewalDate = DateTime.UtcNow.AddYears(1),
        MembershipTypeID = membershipTypeId
    };

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.AddUserAsync(user));
    
    Assert.That(exception.Message, Contains.Substring("email"));
    _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<IUser>()), Times.Never);
}

[Test]
public void AddUserAsync_WithInvalidMembershipTypeId_ShouldThrowArgumentException()
{
    // Arrange
    string firstName = "John";
    string lastName = "Doe";
    string email = "test@example.com";
    string password = "password123";
    int membershipTypeId = 0;  // Invalid membership type ID
    bool isActive = true;
    
    var user = new User
    {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = password,
        IsActive = isActive,
        RegistrationDate = DateTime.UtcNow,
        RenewalDate = DateTime.UtcNow.AddYears(1),
        MembershipTypeID = membershipTypeId
    };

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.AddUserAsync(user));
    
    Assert.That(exception.Message, Contains.Substring("Lidmaatschapstype"));
    _mockUserRepository.Verify(repo => repo.AddUserAsync(It.IsAny<IUser>()), Times.Never);
}

[Test]
public async Task DeactivateUserAsync_ShouldUpdateUser()
{
    // Arrange
    int userId = 1;
    string firstName = "John";
    string lastName = "Doe";
    string email = "john@example.com";
    string passwordHash = "hashedpassword";
    bool isActive = true;
    
    var user = new User {        
        Id = userId, 
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = passwordHash,
        IsActive = isActive
    };
    
    var users = new List<User> { user };
    _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users.Cast<IUser>().ToList());

    // Act
    await _userService.DeactivateUserAsync(userId);

    // Assert
    _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.Is<IUser>(u => 
        u.Id == userId && 
        u.IsActive == false)), Times.Once);
}

[Test]
public void DeactivateUserAsync_UserNotFound_ShouldThrowArgumentException()
{
    // Arrange
    int existingUserId = 1;
    int nonExistentUserId = 999; // Non-existent user
    
    var existingUser = new User { 
        Id = existingUserId,
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        PasswordHash = "hashedpassword"
    };
    
    var users = new List<User> { existingUser };
    _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users.Cast<IUser>().ToList());

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.DeactivateUserAsync(nonExistentUserId));
    
    Assert.That(exception.Message, Contains.Substring("niet gevonden"));
}

[Test]
public async Task RenewMembershipAsync_ShouldUpdateUser()
{
    // Arrange
    int userId = 1;
    string firstName = "John";
    string lastName = "Doe";
    string email = "john@example.com";
    string passwordHash = "hashedpassword";
    
    var user = new User { 
        Id = userId,
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = passwordHash
    };
    
    var users = new List<User> { user };
    var newRenewalDate = DateTime.UtcNow.AddYears(2);
    _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users.Cast<IUser>().ToList());

    // Act
    await _userService.RenewMembershipAsync(userId, newRenewalDate);

    // Assert
    _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.Is<IUser>(u => 
        u.Id == userId && 
        u.RenewalDate == newRenewalDate &&
        u.IsActive == true)), Times.Once);
}

[Test]
public void RenewMembershipAsync_PastDate_ShouldThrowArgumentException()
{
    // Arrange
    int userId = 1;
    string firstName = "John";
    string lastName = "Doe";
    string email = "john@example.com";
    string passwordHash = "hashedpassword";
    
    var user = new User { 
        Id = userId,
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = passwordHash
    };
    
    var users = new List<User> { user };
    var pastDate = DateTime.UtcNow.AddDays(-1);  // Date in the past
    _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users.Cast<IUser>().ToList());

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.RenewMembershipAsync(userId, pastDate));
    
    Assert.That(exception.Message, Contains.Substring("toekomst"));
}

[Test]
public async Task ChangeMembershipTypeAsync_ShouldUpdateUser()
{
    // Arrange
    int userId = 1;
    string firstName = "John";
    string lastName = "Doe";
    string email = "john@example.com";
    string passwordHash = "hashedpassword";
    int currentMembershipTypeId = 1;
    
    var user = new User { 
        Id = userId, 
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = passwordHash,
        MembershipTypeID = currentMembershipTypeId
    };
    
    var users = new List<User> { user };
    int newMembershipTypeId = 2;
    _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users.Cast<IUser>().ToList());

    // Act
    await _userService.ChangeMembershipTypeAsync(userId, newMembershipTypeId);

    // Assert
    _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.Is<IUser>(u => 
        u.Id == userId && 
        u.MembershipTypeID == newMembershipTypeId)), Times.Once);
}

[Test]
public void ChangeMembershipTypeAsync_InvalidType_ShouldThrowArgumentException()
{
    // Arrange
    int userId = 1;
    string firstName = "John";
    string lastName = "Doe";
    string email = "john@example.com";
    string passwordHash = "hashedpassword";
    int currentMembershipTypeId = 1;
    
    var user = new User { 
        Id = userId, 
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PasswordHash = passwordHash,
        MembershipTypeID = currentMembershipTypeId
    };
    
    var users = new List<User> { user };
    int invalidTypeId = 0;  // Invalid membership type ID
    _mockUserRepository.Setup(repo => repo.GetAllUsersAsync()).ReturnsAsync(users.Cast<IUser>().ToList());

    // Act & Assert
    var exception = Assert.ThrowsAsync<ArgumentException>(async () => 
        await _userService.ChangeMembershipTypeAsync(userId, invalidTypeId));
    
    Assert.That(exception.Message, Contains.Substring("ongeldig"));
}
}