// Services/TodoService.cs (Dapper Metotları Doldurulmuş Hali)

using Dapper;
using Microsoft.Data.SqlClient;
using TodoTimeLineApp_Backend.NET.Models;
using TodoTimeLineApp_Backend.NET.Services;

namespace PersonalTodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly string _connectionString;

        public TodoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private SqlConnection GetOpenConnection()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }


        public async Task<IEnumerable<Todo>> GetCurrentTodosAsync()
        {
            using (var conn = GetOpenConnection())
            {
                const string sql = "SELECT TOP 6 * FROM Todos WHERE IsCurrent = 1 ORDER BY CreatedAt DESC";
                return await conn.QueryAsync<Todo>(sql);
            }
        }

        public async Task<IEnumerable<Todo>> GetNextTodosAsync()
        {
            using (var conn = GetOpenConnection())
            {
                const string sql = "SELECT TOP 6 * FROM Todos WHERE IsCurrent = 0 ORDER BY CreatedAt DESC";
                return await conn.QueryAsync<Todo>(sql);
            }
        }


        public async Task<Todo> AddTodoAsync(string text, bool isCurrent)
        {
            using (var conn = GetOpenConnection())
            {
                const string sql = @"
                    INSERT INTO Todos (Text, IsCurrent, CreatedAt)
                    OUTPUT INSERTED.Id, INSERTED.Text, INSERTED.IsCurrent, INSERTED.CreatedAt
                    VALUES (@Text, @IsCurrent, GETDATE())";

                var newTodo = await conn.QuerySingleAsync<Todo>(sql, new { Text = text, IsCurrent = isCurrent });
                return newTodo;
            }
        }

        public async Task<Todo> MoveToCurrentAsync(int id)
        {
            using (var conn = GetOpenConnection())
            {
                const string updateSql = @"
                    UPDATE Todos 
                    SET IsCurrent = 1, CreatedAt = GETDATE() 
                    WHERE Id = @Id AND IsCurrent = 0";

                var affectedRows = await conn.ExecuteAsync(updateSql, new { Id = id });

                if (affectedRows == 0)
                {
                    return null;
                }

                const string selectSql = "SELECT * FROM Todos WHERE Id = @Id";
                return await conn.QuerySingleOrDefaultAsync<Todo>(selectSql, new { Id = id });
            }
        }


        public async Task<bool> DeleteTodoAsync(int id)
        {
            using (var conn = GetOpenConnection())
            {
                const string sql = "DELETE FROM Todos WHERE Id = @Id";
                var affectedRows = await conn.ExecuteAsync(sql, new { Id = id });

                return affectedRows > 0;
            }
        }
    }
}