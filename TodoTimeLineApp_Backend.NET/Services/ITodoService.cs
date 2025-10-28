using TodoTimeLineApp_Backend.NET.Models;

namespace TodoTimeLineApp_Backend.NET.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetCurrentTodosAsync();
        Task<IEnumerable<Todo>> GetNextTodosAsync();
        Task<Todo> AddTodoAsync(string text, bool isCurrent);
        Task<bool> DeleteTodoAsync(int id);
        Task<Todo> MoveToCurrentAsync(int id);
    }
}
