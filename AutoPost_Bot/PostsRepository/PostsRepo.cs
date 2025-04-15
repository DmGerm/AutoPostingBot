using AutoPost_Bot.Models;

namespace AutoPost_Bot.PostsRepository
{
    public class PostsRepo : IPostsRepo
    {
        private readonly List<PostModel> _posts = [];

        public Task<PostModel?> AddPostAsync(PostModel post)
        {
            try
            {
                _posts.Add(post);
                return Task.FromResult<PostModel?>(post);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Task.FromResult<PostModel?>(null);
            }
        }

        public Task<PostModel?> ChangePostByIdAsync(Guid id, PostModel post)
        {
            throw new NotImplementedException();
        }

        public Task<PostModel?> GetPostByIdAsync(Guid id) { throw new NotImplementedException(); }

        public Task<List<PostModel>> GetPostsAsync() => Task.FromResult(_posts);
    }
}
