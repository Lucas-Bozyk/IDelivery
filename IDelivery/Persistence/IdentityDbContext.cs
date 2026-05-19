using IDelivery.Domain;
using Microsoft.EntityFrameworkCore;

namespace IDelivery.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRoleMap> UserRoles => Set<UserRoleMap>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserRoleMap>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<UserRoleMap>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);
        modelBuilder.Entity<UserRoleMap>()
            .HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId);
        modelBuilder.Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId);
    }
}
