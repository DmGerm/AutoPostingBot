using AutoPost_Bot.Models;

namespace AutoPost_Bot.PostsRepository;

public interface IPostsRepo
{
    public Task<List<PostModel>> GetPostsAsync();
    public Task<PostModel?> GetPostByIdAsync(Guid id);
    public Task<PostModel?> ChangePostByIdAsync(Guid id, PostModel post);
    public Task<PostModel?> AddPostAsync(PostModel post);
    public Task SavePostChangesAsync(List<PostModel> postsList);
    public Task UpdatePostAsync(PostModel post);
    public Task<List<PostModel>> GetPostsByBotTokenAsync(string botToken);
}