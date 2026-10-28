using Microsoft.Data.SqlClient;

namespace AnimalShelterAPI
{
    public static class DbHelper
    {
        private static string _connectionString;

        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}