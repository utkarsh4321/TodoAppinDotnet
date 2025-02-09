using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todoApis.Models;


namespace todoApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly PasswordHasher<LoginDTO> _passwordHasher = new PasswordHasher<LoginDTO>();
        private readonly TokenService _tokenService;

        public AuthController(TodoContext context,TokenService tokenSerice)
        {
            _context = context;
            _tokenService = tokenSerice;
        }

        //[Route("login")]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDTO user)
        {
            if (user.Email == null || user.Password == null) {

                return BadRequest("email and password are required");
            }
            var IsUserExist = await _context.Users.Where(b => b.Email == user.Email).ToListAsync();
            if (IsUserExist == null || IsUserExist.Count == 0)
            {
                return BadRequest("Username and password wrong");
            }
            var storedUser = IsUserExist[0];
            var result = _passwordHasher.VerifyHashedPassword(user, storedUser.Password, user.Password);
            if (result == PasswordVerificationResult.Success) {
                var token = _tokenService.GenerateToken(user);
                return Ok(new { Token = token });
            }
            else
            {
                return BadRequest("Password is wrong please enter correcy password");
            }
            
        }
        //[Route("register")]
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(LoginDTO user) {
            var IsUserExist = await _context.Users.Where(b=>b.Email == user.Email).ToListAsync();
            if (IsUserExist != null && IsUserExist.Count > 0)
            {
                return BadRequest("User already exists");
            }
            var MyUser = new User
            {
                Email = user.Email,
                Password = user.Password,
                Username = user.UserName,
            };
            MyUser.Password = _passwordHasher.HashPassword(user, user.Password);
           
            _context.Users.Add(MyUser);
            await _context.SaveChangesAsync();

            return Ok(new UserDTO
            {
                Id = MyUser.Id,
                UserName = user.UserName,
                UserEmail = user.Email
            });
        }
    }
}
