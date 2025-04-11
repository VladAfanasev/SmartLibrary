using MySql.Data.MySqlClient;
using System.Data;

namespace SmartLibrary.Infrastructure.Data;

public class MySqlDatabase
{
    private readonly string _connectionString;

    public MySqlDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = GetConnection();
        using var command = new MySqlCommand(query, connection);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        var dataTable = new DataTable();
        using var adapter = new MySqlDataAdapter(command);
        adapter.Fill(dataTable);

        return dataTable;
    }

    public int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = GetConnection();
        connection.Open();
        using var command = new MySqlCommand(query, connection);
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }

        return command.ExecuteNonQuery();
    }
}