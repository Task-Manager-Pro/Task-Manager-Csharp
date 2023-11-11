using Microsoft.EntityFrameworkCore;
using Todo.Models;

namespace Todo.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CategorieTaskEntity> CategorieTasks { get; set; }
    }
}