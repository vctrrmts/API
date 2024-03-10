using Common.Domain;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class ApplicationDbContext : DbContext
{
    public DbSet<ToDo> ToDos { get; set; }

    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDo>().HasKey(c => c.Id);
        modelBuilder.Entity<ToDo>().Property(b => b.Label).HasMaxLength(100).IsRequired();

        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(b => b.Name).HasMaxLength(50).IsRequired();
     
        modelBuilder.Entity<ToDo>()
            .HasOne(v => v.User)
            .WithMany(c => c.ToDo)
            .HasForeignKey(v => v.OwnerId);

        base.OnModelCreating(modelBuilder);
    }
}