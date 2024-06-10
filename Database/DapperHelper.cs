using Dapper;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace net6_wpf_webview2_template.Database
{
    public class DapperHelper
    {
        public int CommandTimeout { get; set; } = 30;
        public string ConnectionString { get; set; }
        public DatabaseType DatabaseType { get; set; }

        private IDbConnection OpenConnection()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new ArgumentNullException("数据库连接字符串不能为空");
            }

            IDbConnection connection = DatabaseType switch
            {
                DatabaseType.MYSQL => new MySqlConnection(ConnectionString),
                DatabaseType.POSTGRE => new NpgsqlConnection(ConnectionString),
                _ => throw new Exception("数据库类型不支持"),
            };

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection;
        }

        public async Task<bool> TestConnect()
        {
            return await QueryOne<int>("select 1;") == 1;
        }

        public async Task<IList<SchemaTable>> AllTables()
        {
            return await Query<SchemaTable>("select c.relname as TableName, d.description as TableComment from pg_description d join pg_class c on d.objoid = c.oid where c.relkind = 'r' and d.objsubid = 0;");
        }

         public async Task<IList<SchemaColumn>> AllColumns(string tableName)
        {
            return await Query<SchemaColumn>($"SELECT cols.column_name ColumnName, cols.data_type ColumnType, pgd.description as ColumnComment FROM information_schema.columns cols LEFT JOIN pg_catalog.pg_description pgd ON pgd.objsubid = cols.ordinal_position AND pgd.objoid = (SELECT c.oid FROM pg_catalog.pg_class c WHERE c.relname = cols.table_name) WHERE cols.table_name = '{tableName}';");
        }

        public async Task<T> QueryOne<T>(string sql, object? condition = null)
        {
            using IDbConnection conn = OpenConnection();
            var res = await conn.QueryFirstOrDefaultAsync<T>(sql, condition, commandTimeout: CommandTimeout);
            conn.Close();
            return res;
        }

        public async Task<IList<T>> Query<T>(string sql, object? condition = null)
        {
            using IDbConnection conn = OpenConnection();
            var res = (await conn.QueryAsync<T>(sql, condition, commandTimeout: CommandTimeout)).ToList();
            conn.Close();
            return res;
        }

    }

}
