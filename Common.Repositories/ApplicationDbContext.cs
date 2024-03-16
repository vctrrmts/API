using Common.Domain;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class ApplicationDbContext : DbContext
{
    public DbSet<ToDo> ToDos { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDo>().HasKey(t => t.Id);
        modelBuilder.Entity<ToDo>().Property(t => t.Label).HasMaxLength(100).IsRequired();

        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<User>().Property(u => u.Login).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();

        modelBuilder.Entity<ToDo>()
            .HasOne(v => v.User)
            .WithMany(c => c.ToDos)
            .HasForeignKey(v => v.OwnerId);

        modelBuilder.Entity<UserRole>().HasKey(u => u.Id);
        modelBuilder.Entity<UserRole>().Property(u => u.Name).HasMaxLength(50).IsRequired();

        modelBuilder.Entity<User>()
            .HasOne(v => v.UserRole)
            .WithMany(v => v.Users)
            .HasForeignKey(c => c.UserRoleId);

        modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);
        modelBuilder.Entity<RefreshToken>().Property(r => r.Id).HasDefaultValueSql("NEWID()");
        modelBuilder.Entity<RefreshToken>()
            .HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId);

        base.OnModelCreating(modelBuilder);
    }
}