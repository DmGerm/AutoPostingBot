using AutoMapper;
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.PostsRepository
{
    public class PostsRepo(PostsContext postsContext, IMapper mapper) : IPostsRepo
    {
        private readonly PostsContext _postsContext = postsContext;
        private readonly IMapper _mapper = mapper;

        public async Task<PostModel?> AddPostAsync(PostModel post)
        {
            try
            {
                _postsContext.Posts.Add(post);
                await _postsContext.SaveChangesAsync();
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
            return await _postsContext.Posts
                                      .FirstOrDefaultAsync(post => post.Id == id);
        }

        public async Task<List<PostModel>> GetPostsAsync() => await _postsContext.Posts.ToListAsync();


        public async Task SavePostChangesAsync(List<PostModel> postsList)
        {
            try
            {
                _postsContext.ChangeTracker.Clear();
                var existing = await _postsContext.Posts.ToListAsync();
                _postsContext.Posts.RemoveRange(existing);
                await _postsContext.AddRangeAsync(postsList);
                await _postsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при сохранении: " + ex.Message);
                Console.WriteLine("InnerException: " + ex.InnerException?.Message);
            }
        }

        public async Task UpdatePostAsync(PostModel post)
        {
            var dbPost = await _postsContext.Posts.FindAsync(post.Id);

            if (dbPost != null)
            {
                _mapper.Map(dbPost, post);
            }
            try
            {
                await _postsContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
