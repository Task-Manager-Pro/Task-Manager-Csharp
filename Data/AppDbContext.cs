using Microsoft.EntityFrameworkCore;
using Todo.Domain;

namespace Todo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CategorieTaskEntity> CategorieTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskEntity>()
                .Property(x => x.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<TaskEntity>()
                .Property(x => x.State)
                .HasConversion<string>();
        }
    }
}
