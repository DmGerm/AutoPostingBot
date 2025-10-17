using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.PostsRepository;

public class PostsRepo(PostsContext postsContext) : IPostsRepo
{
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