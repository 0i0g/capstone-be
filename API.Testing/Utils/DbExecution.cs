using System;
using System.Collections;
using Microsoft.Data.SqlClient;

namespace API.Testing
{
    public class DbExecution : IDisposable
    {
        private static readonly string _connectionString =
            "Data Source=.;Initial Catalog=ERPSystem;Integrated Security=True";

        private SqlConnection Connection { get; }

        public SqlCommand Command { get; }


        public DbExecution(string query)
        {
            Connection = new SqlConnection(_connectionString);
            Command = new SqlCommand(query, Connection);
            Connection.Open();
        }

        public void Dispose()
        {
            Connection.Dispose();
            Command.Dispose();
        }
        
        // public static T ExecuteScalar<T>(string sql)
        // {
        //     using var connection = new SqlConnection(GetConnectionString());
        //     using var command = new SqlCommand(sql, connection);
        //     connection.Open();
        //     var a = command.ExecuteScalar();
        //     return (T) command.ExecuteScalar();
        // }
    }
}