using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todoApis.Models;

namespace todoApis.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDTO>>> GetTodos()
        {
            return await _context.Todos.Include(t => t.User).Select(x=>ItemToDTO(x)).ToListAsync();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDTO>> GetTodo(int id)
        {
            var todo = await _context.Todos.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            return ItemToDTO(todo);
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoDTO>> PostTodo(Todo todo)
        {
            var user = await _context.Users.FindAsync(todo.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, ItemToDTO(todo));
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodo(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest();
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Todos.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        private static TodoDTO ItemToDTO(Todo todo) => new TodoDTO
        {
            Id = todo.Id,
            TodoName = todo.TodoName,
            Status = todo.Status,
            userId = todo.UserId,
        };
    }
}