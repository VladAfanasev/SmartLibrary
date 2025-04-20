using System.Data;
using SmartLibrary.Application.Interfaces;
using SmartLibrary.Domain.Interfaces;
using SmartLibrary.Domain.Entities;
using SmartLibrary.Infrastructure.Data;
using Microsoft.Extensions.Logging;
namespace SmartLibrary.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<IUser>> GetAllUsersAsync()
    {
        const string query = "SELECT * FROM Users";
        var dataTable = await Task.Run(() => _dbContext.ExecuteQuery(query));

        var users = new List<IUser>();
        foreach (DataRow row in dataTable.Rows)
        {
            users.Add(MapUserFromDataRow(row));
        }

        return users;
    }
    
    public async Task<IUser> GetUserByIdAsync(int userId)
    {
        const string query = "SELECT * FROM Users WHERE Id = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "@Id", userId }
        };
    
        var dataTable = await Task.Run(() => _dbContext.ExecuteQuery(query, parameters));
    
        if (dataTable.Rows.Count == 0)
            return null;
        
        return MapUserFromDataRow(dataTable.Rows[0]);
    }
    
    public async Task<IUser> GetUserByEmailAsync(string email)
    {
        const string query = "SELECT * FROM Users WHERE Email = @Email";
        var parameters = new Dictionary<string, object>
        {
            { "@Email", email }
        };
        
        var dataTable = await Task.Run(() => _dbContext.ExecuteQuery(query, parameters));
        
        if (dataTable.Rows.Count == 0)
            return null;
            
        return MapUserFromDataRow(dataTable.Rows[0]);
    }
    
    public async Task AddUserAsync(IUser user)
    {
        _logger.LogInformation("AddUserAsync called with user: {Email}", user.Email);
        try
        {
            const string query = @"
            INSERT INTO Users (Email, PasswordHash, FirstName, LastName, IsActive, RegistrationDate, RenewalDate, MembershipTypeID)
            VALUES (@Email, @PasswordHash, @FirstName, @LastName, @IsActive, @RegistrationDate, @RenewalDate, @MembershipTypeID)";

            var parameters = new Dictionary<string, object>
            {
                { "@Email", user.Email },
                { "@PasswordHash", user.PasswordHash },
                { "@FirstName", user.FirstName },
                { "@LastName", user.LastName },
                { "@IsActive", user.IsActive },
                { "@RegistrationDate", user.RegistrationDate },
                { "@RenewalDate", user.RenewalDate },
                { "@MembershipTypeID", user.MembershipTypeID }
            };

            await Task.Run(() => _dbContext.ExecuteNonQuery(query, parameters));
            _logger.LogInformation("User added to database successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddUserAsync.");
            throw;
        }
    }
    
    public async Task UpdateUserAsync(IUser user)
    {
        var domainUser = (User)user; // Cast to domain entity

        const string query = @"
        UPDATE Users 
        SET FirstName = @FirstName, 
            LastName = @LastName, 
            Email = @Email, 
            PasswordHash = @PasswordHash,
            IsActive = @IsActive,
            RenewalDate = @RenewalDate,
            MembershipTypeID = @MembershipTypeID
        WHERE Id = @Id";
        
        var parameters = new Dictionary<string, object>
        {
            { "@Id", domainUser.Id },
            { "@FirstName", domainUser.FirstName },
            { "@LastName", domainUser.LastName },
            { "@Email", domainUser.Email },
            { "@PasswordHash", domainUser.PasswordHash },
            { "@IsActive", domainUser.IsActive },
            { "@RenewalDate", domainUser.RenewalDate },
            { "@MembershipTypeID", domainUser.MembershipTypeID }
        };

        await Task.Run(() => _dbContext.ExecuteNonQuery(query, parameters));
    }
    
    public async Task DeleteUserAsync(int userId)
    {
        const string query = "DELETE FROM Users WHERE Id = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "@Id", userId }
        };

        await Task.Run(() => _dbContext.ExecuteNonQuery(query, parameters));
    }
    
    public async Task DeleteUserAsync(IUser user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        _logger.LogInformation("DeleteUserAsync called for user: {UserId}, {Email}", user.Id, user.Email);
        
        await DeleteUserAsync(user.Id);
    }
    
    // Helper method for mapping users
    private IUser MapUserFromDataRow(DataRow row)
    {
        return new User
        {
            Id = Convert.ToInt32(row["Id"]),
            FirstName = row["FirstName"].ToString(),
            LastName = row["LastName"].ToString(),
            Email = row["Email"].ToString(),
            PasswordHash = row["PasswordHash"].ToString(),
            IsActive = Convert.ToBoolean(row["IsActive"]),
            RegistrationDate = Convert.ToDateTime(row["RegistrationDate"]),
            RenewalDate = Convert.ToDateTime(row["RenewalDate"]),
            MembershipTypeID = Convert.ToInt32(row["MembershipTypeID"])
        };
    }
}