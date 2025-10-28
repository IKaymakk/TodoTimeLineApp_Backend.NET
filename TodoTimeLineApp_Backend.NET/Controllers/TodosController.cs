using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoTimeLineApp_Backend.NET.Models;
using TodoTimeLineApp_Backend.NET.Services;

namespace TodoTimeLineApp_Backend.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodosController(ITodoService todoService)
        {
            _todoService = todoService;
        }

       

        [HttpGet("current")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetCurrentTodos()
        {
            var todos = await _todoService.GetCurrentTodosAsync();
            return Ok(todos);
        }

        [HttpGet("next")]
        public async Task<ActionResult<IEnumerable<Todo>>> GetNextTodos()
        {
            var todos = await _todoService.GetNextTodosAsync();
            return Ok(todos);
        }


        [HttpPost("current")]
        public async Task<ActionResult<Todo>> AddCurrentTodo([FromBody] TodoInputDto input)
        {
            // true = IsCurrent
            var todo = await _todoService.AddTodoAsync(input.Text, true);
            return CreatedAtAction(nameof(GetCurrentTodos), new { id = todo.Id }, todo);
        }

        [HttpPost("next")]
        public async Task<ActionResult<Todo>> AddNextTodo([FromBody] TodoInputDto input)
        {
            var todo = await _todoService.AddTodoAsync(input.Text, false);
            return CreatedAtAction(nameof(GetNextTodos), new { id = todo.Id }, todo);
        }


        [HttpPost("move/{id}")]
        public async Task<ActionResult<Todo>> MoveTodo(int id)
        {
            var updatedTodo = await _todoService.MoveToCurrentAsync(id);

            if (updatedTodo == null)
            {
                return NotFound($"ID {id} ile Next listesinde taşınacak görev bulunamadı.");
            }

            return Ok(updatedTodo);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var success = await _todoService.DeleteTodoAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent(); 
        }
    }
}
