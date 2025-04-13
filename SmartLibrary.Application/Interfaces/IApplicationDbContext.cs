using System.Data;

namespace SmartLibrary.Application.Interfaces;

public interface IApplicationDbContext
{
    DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null);
    int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null);
}