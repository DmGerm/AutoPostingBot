using AutoMapper;
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.PostsRepository;

public class PostsRepo(PostsContext postsContext, IMapper mapper) : IPostsRepo
{
    public async Task<PostModel?> AddPostAsync(PostModel post)
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


    public async Task SavePostChangesAsync(List<PostModel> postsList)
    {
        try
        {
            postsContext.ChangeTracker.Clear();
            var existing = await postsContext.Posts.ToListAsync();
            postsContext.Posts.RemoveRange(existing);
            await postsContext.AddRangeAsync(postsList);
            await postsContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении: " + ex.Message);
            Console.WriteLine("InnerException: " + ex.InnerException?.Message);
        }
    }

    public async Task UpdatePostAsync(PostModel post)
    {
        var dbPost = await postsContext.Posts.FindAsync(post.Id);

        if (dbPost != null) mapper.Map(dbPost, post);
        try
        {
            await postsContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task<List<PostModel>> GetPostsByBotTokenAsync(string botToken)
    {
        try
        {
            return await postsContext.Posts.Where(post => post.BotID != null && post.BotID.Equals(botToken))
                .ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}