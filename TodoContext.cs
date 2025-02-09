using Microsoft.EntityFrameworkCore;
using todoApis.Models;

namespace todoApis
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }
    
    public DbSet<Todo> Todos { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
