using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.Data;

public class UserContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<RoleModel> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserModel>(users =>
            {
                users.HasKey(u => u.UserId)
                    .HasName("UserId");

                users.HasIndex(u => u.UserId)
                    .IsUnique();

                users.HasIndex(u => u.Email);

                users.Property(u => u.UserId)
                    .HasColumnType("TEXT")
                    .ValueGeneratedNever();

                users.HasOne(u => u.Role)
                    .WithMany(role => role.Users)
                    .HasForeignKey(u => u.RoleId);
            }
        );

        modelBuilder.Entity<RoleModel>(role =>
            {
                role.HasKey(roleModel => roleModel.RoleId)
                    .HasName("RoleId");

                role.Property(roleModel => roleModel.RoleId)
                    .ValueGeneratedNever();

                role.HasData(new RoleModel { RoleId = RoleId.Root });
                role.HasData(new RoleModel { RoleId = RoleId.Administrator });
                role.HasData(new RoleModel { RoleId = RoleId.User });
            }
        );
    }
}