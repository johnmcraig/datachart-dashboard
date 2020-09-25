﻿using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DataDashboard.Core.DataSqlAccess;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DataDashboard.Infrastructure.DataAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }
        
        public async Task<List<T>> LoadData<T, U>(string sql, U parameters, string connectionStringName)
        {
            using (IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                var rows = await connection.QueryAsync<T>(sql, parameters, commandType: CommandType.Text);

                return rows.ToList();
            }
        }

        public async Task SaveData<T>(string sql, T parameters, string connectionStringName)
        {
            using (IDbConnection connection = new NpgsqlConnection(_config.GetConnectionString(connectionStringName)))
            {
                connection.Open();

                await connection.ExecuteAsync(sql, parameters, commandType: CommandType.Text);
            }
        }
    }
}