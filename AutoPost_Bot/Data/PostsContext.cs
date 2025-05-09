using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.Data
{
    public class PostsContext(DbContextOptions<PostsContext> options) : DbContext(options)
    {
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<GroupModel> Groups { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostModel>(post =>
            {
                post.HasKey(x => x.Id).HasName("Post_Id");
                post.Property(x => x.Id).HasColumnType("TEXT")
                                        .ValueGeneratedNever();
                post.HasIndex(x => x.Id).HasDatabaseName("IX_Posts_Id");
                post.HasOne(p => p.Group)
                    .WithMany(x => x.Posts)
                    .HasForeignKey(x => x.GroupID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<GroupModel>(group =>
            {
                group.HasKey(x => x.GroupId).HasName("Group_Id");
                group.Property(x => x.GroupId)
                     .ValueGeneratedNever();
            });
        }
    }
}
