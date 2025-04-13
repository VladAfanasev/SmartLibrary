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

        // Act
        await _userService.RegisterUserAsync(firstname, lastname, email, password);

        // Assert
        _mockUserRepository.Verify(repo => repo.AddUserAsync(It.Is<User>(u =>
            u.FirstName == firstname &&
            u.LastName == lastname &&
            u.Email == email &&
            BCrypt.Net.BCrypt.Verify(password, u.PasswordHash, false, BCrypt.Net.HashType.SHA384)
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
}