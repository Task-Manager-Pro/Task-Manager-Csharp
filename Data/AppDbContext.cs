using Microsoft.EntityFrameworkCore;
using Todo.Models;

namespace Todo.Data {
    public class AppDbContext : DbContext {
       // public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TodoModel> TodoItems { get; set; }

        public DbSet<TodoModelPlus> TodoItemsPlus { get; set; }

        public DbSet<LoginModel> Login { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=app.db;Cache=Shared");
        }
    }
}