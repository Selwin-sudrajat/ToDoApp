using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Models
{
    public class ToDoDbContext : DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options) { }

        public virtual DbSet<ToDo> ToDo { get; set; }
        public DbSet<Log> Log { get; set; }
    }
}