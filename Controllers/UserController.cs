using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todoApis.Models;
using todoApis;
using Microsoft.AspNetCore.Authorization;

namespace todoApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TodoContext _context;

        public UserController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            // Include Todos for each user
            return await _context.Users
                .Include(u => u.Todos).Select(x =>GenUserDTO(x)) // Include the Todos navigation property
                .ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            // Include Todos for the specific user
            var user = await _context.Users
                .Include(u => u.Todos).Select(x => GenUserDTO(x)) // Include the Todos navigation property
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(User user)
        {
            var IsUserExist =   await _context.Users.FindAsync(user.Id);
            if (IsUserExist != null) {
                return BadRequest("User already exists");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDTO
            {
                Id = user.Id,
               UserName = user.Username,
               UserEmail = user.Email,
            });
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
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

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/User/5/Todos
        [HttpGet("{id}/Todos")]
        public async Task<ActionResult<IEnumerable<TodoDTO>>> GetUserTodos(int id)
        {
            // Check if the user exists
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch todos for the user
            var todos = await _context.Todos
                .Where(t => t.UserId == id).Select(x=>ItemToDTO(x))
                .ToListAsync();

            return Ok(todos);
        }
        private static UserDTO GenUserDTO(User user)
        {
            UserDTO UserDto = new UserDTO
            {
                Id = user.Id,
                UserName = user.Username,
                UserEmail = user.Email,

            };
            var customTodoList = new List<TodoDTO>();
            foreach (var t in user.Todos)
            {
                TodoDTO TodoItem =  ItemToDTO(t);
                customTodoList.Add(TodoItem);
            }
            UserDto.Todos = customTodoList;
            return UserDto;
        }
        
        private static TodoDTO ItemToDTO(Todo todoItem) =>
      new TodoDTO
      {
          Id = todoItem.Id,
          TodoName = todoItem.TodoName,
          Status = todoItem.Status,
          userId = todoItem.UserId,
      };
    }
}