using System.Data;
using SmartLibrary.Application.Interfaces;
using SmartLibrary.Domain.Entities;
using SmartLibrary.Infrastructure.Data;

namespace SmartLibrary.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddUserAsync(User user)
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
    }

    public async Task<List<User>> GetUsersAsync()
    {
        const string query = "SELECT * FROM Users";
        var dataTable = await Task.Run(() => _dbContext.ExecuteQuery(query));

        var users = new List<User>();
        foreach (DataRow row in dataTable.Rows)
        {
            users.Add(new User
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
            });
        }

        return users;
    }

    public async Task UpdateUserAsync(User user)
    {
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
            { "@Id", user.Id },
            { "@FirstName", user.FirstName },
            { "@LastName", user.LastName },
            { "@Email", user.Email },
            { "@PasswordHash", user.PasswordHash },
            { "@IsActive", user.IsActive },
            { "@RenewalDate", user.RenewalDate },
            { "@MembershipTypeID", user.MembershipTypeID }
        };

        await Task.Run(() => _dbContext.ExecuteNonQuery(query, parameters));
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        const string query = "SELECT * FROM Users WHERE Id = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "@Id", userId }
        };
        
        var dataTable = await Task.Run(() => _dbContext.ExecuteQuery(query, parameters));
        
        if (dataTable.Rows.Count == 0)
            return null;
            
        var row = dataTable.Rows[0];
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

    public async Task<User> GetUserByEmailAsync(string email)
    {
        const string query = "SELECT * FROM Users WHERE Email = @Email";
        var parameters = new Dictionary<string, object>
        {
            { "@Email", email }
        };
        
        var dataTable = await Task.Run(() => _dbContext.ExecuteQuery(query, parameters));
        
        if (dataTable.Rows.Count == 0)
            return null;
            
        var row = dataTable.Rows[0];
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

    public async Task DeleteUserAsync(int userId)
    {
        const string query = "DELETE FROM Users WHERE Id = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "@Id", userId }
        };

        await Task.Run(() => _dbContext.ExecuteNonQuery(query, parameters));
    }
}