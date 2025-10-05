using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.Data
{
    public class PostsContext(DbContextOptions<PostsContext> options) : DbContext(options)
    {
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<GroupModel> Groups { get; set; }
        public DbSet<BotModel> Bots { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BotModel>(bot =>
            {
                bot.HasKey(x => x.Token).HasName("Bot_token");

                bot.Property(x => x.Token).HasColumnType("TEXT")
                                      .ValueGeneratedNever();

                bot.HasIndex(x => x.Token).IsUnique().HasDatabaseName("IX_Bots_Token");

                bot.HasMany(x => x.Groups)
                    .WithMany(x => x.Bots)
                    .UsingEntity(j => j.ToTable("BotGroups"));

                bot.HasMany(b => b.Posts)
                   .WithOne(p => p.Bot)
                   .HasForeignKey(p => p.BotToken)
                   .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PostModel>(post =>
            {
                post.HasKey(x => x.Id).HasName("Post_Id");

                post.Property(x => x.Id).HasColumnType("TEXT")
                                        .ValueGeneratedNever();

                post.HasIndex(x => x.Id).HasDatabaseName("IX_Posts_Id");
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
