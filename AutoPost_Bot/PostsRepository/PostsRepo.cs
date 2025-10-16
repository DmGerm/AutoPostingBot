using AutoMapper;
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.PostsRepository;

public class PostsRepo(PostsContext postsContext, IMapper mapper) : IPostsRepo
{
    /*    public async Task<PostModel?> AddPostAsync(PostModel post)
        {
            try
            {
                postsContext.Posts.Add(post);
                await postsContext.SaveChangesAsync();
                return post;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Task<PostModel?> ChangePostByIdAsync(Guid id, PostModel post)
        {
            throw new NotImplementedException();
        }

        public async Task<PostModel?> GetPostByIdAsync(Guid id)
        {
            return await postsContext.Posts
                .FirstOrDefaultAsync(post => post.Id == id);
        }

        public async Task<List<PostModel>> GetPostsAsync()
        {
            return await postsContext.Posts.ToListAsync();
        }


        public async Task UpdatePostAsync(PostModel post)
        {
            var dbPost = await postsContext.Posts.FindAsync(post.Id);

            if (dbPost != null) mapper.Map(post, dbPost);
            try
            {
                await postsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    */

    public async Task<List<PostModel>> GetPostsByBotTokenAsync(string botToken)
    {
        try
        {
            BotModel? bot = await postsContext.Bots
                .Include(b => b.Groups)
                .Include(b => b.Posts)
                .FirstOrDefaultAsync(b => b.Token != null && b.Token.Trim() == botToken.Trim());

            if (bot?.Posts != null)
            {
                return bot.Posts.ToList();
            }
            else
            {
                throw new Exception("Posts not found");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}