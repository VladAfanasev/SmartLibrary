using MySql.Data.MySqlClient;
using System.Data;
using SmartLibrary.Application.Interfaces;

namespace SmartLibrary.Infrastructure.Data;

public class ApplicationDbContext : IApplicationDbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }
        _connectionString = connectionString;
    }
    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be null or empty.", nameof(query));
        }

        var dataTable = new DataTable();

        try
        {
            using var connection = GetConnection();
            using var command = new MySqlCommand(query, connection);
            AddParameters(command, parameters);

            using var adapter = new MySqlDataAdapter(command);
            adapter.Fill(dataTable);
        }
        catch (Exception ex)
        {
            // Log the exception (use a logging framework or custom logging)
            throw new DataException("An error occurred while executing the query.", ex);
        }

        return dataTable;
    }

    public int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be null or empty.", nameof(query));
        }

        try
        {
            using var connection = GetConnection();
            connection.Open();
            using var command = new MySqlCommand(query, connection);
            AddParameters(command, parameters);

            return command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Log the exception (use a logging framework or custom logging)
            throw new DataException("An error occurred while executing the non-query command.", ex);
        }
    }
    private void AddParameters(MySqlCommand command, Dictionary<string, object>? parameters)
    {
        if (parameters == null) return;

        foreach (var param in parameters)
        {
            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
        }
    }
}