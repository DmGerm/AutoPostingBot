using AutoPost_Bot.Models;

namespace AutoPost_Bot.PostsRepository;

public interface IPostsRepo
{
    public Task<List<PostModel>> GetPostsByBotTokenAsync(string botToken);
}