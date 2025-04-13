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
        const string query = "INSERT INTO Users (FirstName, Lastname, Email, PasswordHash, CreatedAt) VALUES (@FirstName, @LastName, @Email, @PasswordHash, @CreatedAt)";
        var parameters = new Dictionary<string, object>
        {
            { "@Firstname", user.FirstName },
            { "@Lastname", user.LastName },
            { "@Email", user.Email },
            { "@PasswordHash", user.PasswordHash },
            { "@CreatedAt", user.CreatedAt }
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
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            });
        }

        return users;
    }

    public async Task UpdateUserAsync(User user)
    {
        const string query = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PasswordHash = @PasswordHash WHERE Id = @Id";
        var parameters = new Dictionary<string, object>
        {
            { "@Id", user.Id },
            { "@FirstName", user.FirstName },
            { "@Lastname", user.LastName },
            { "@Email", user.Email },
            { "@PasswordHash", user.PasswordHash }
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
}