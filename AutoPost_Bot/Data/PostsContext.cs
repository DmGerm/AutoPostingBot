using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.Data
{
    public class PostsContext(DbContextOptions<PostsContext> options): DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
