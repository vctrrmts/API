using Common.Domain;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories;

public class ApplicationDbContext : DbContext
{
    public DbSet<ToDo> ToDos { get; set; }

    public DbSet<ApplicationUser> Users { get; set; }

    public DbSet<ApplicationUserRole> UserRoles { get; set; }

    public DbSet<ApplicationUserApplicationRole> ApplicationUserApplicationRoles { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDo>().HasKey(t => t.Id);
        modelBuilder.Entity<ToDo>().Property(t => t.Label).HasMaxLength(100).IsRequired();

        modelBuilder.Entity<ApplicationUser>().HasKey(u => u.Id);
        modelBuilder.Entity<ApplicationUser>().Property(u => u.Login).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Login).IsUnique();
        modelBuilder.Entity<ApplicationUser>().Navigation(u => u.Roles).AutoInclude();

        modelBuilder.Entity<ToDo>()
            .HasOne(v => v.User)
            .WithMany(c => c.ToDos)
            .HasForeignKey(v => v.OwnerId);

        modelBuilder.Entity<ApplicationUserRole>().HasKey(u => u.Id);
        modelBuilder.Entity<ApplicationUserRole>().Property(u => u.Name).HasMaxLength(50).IsRequired();

        modelBuilder.Entity<ApplicationUserApplicationRole>().HasKey(u => new
        {
            u.ApplicationUserId, u.ApplicationUserRoleId }
        );
        modelBuilder.Entity<ApplicationUserApplicationRole>().Navigation(u => u.ApplicationUserRole).AutoInclude();

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(v => v.Roles)
            .WithOne(v => v.ApplicationUser)
            .HasForeignKey(c => c.ApplicationUserId);

        modelBuilder.Entity<ApplicationUserRole>()
            .HasMany(v => v.Users)
            .WithOne(v => v.ApplicationUserRole)
            .HasForeignKey(c => c.ApplicationUserRoleId);

        modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);
        modelBuilder.Entity<RefreshToken>().Property(r => r.Id).HasDefaultValueSql("NEWID()");
        modelBuilder.Entity<RefreshToken>()
            .HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId);

        base.OnModelCreating(modelBuilder);
    }
}