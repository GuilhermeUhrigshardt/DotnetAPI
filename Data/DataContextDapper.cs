using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _configuration;

        public DataContextDapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            SqlCommand commandWithParams = new SqlCommand(sql);

            foreach (SqlParameter parameter in parameters)
            {
                commandWithParams.Parameters.Add(parameter);
            }

            SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            sqlConnection.Open();

            commandWithParams.Connection = sqlConnection;

            int rowsAffected = commandWithParams.ExecuteNonQuery();
            sqlConnection.Close();

            return rowsAffected > 0;
        }
    }
}