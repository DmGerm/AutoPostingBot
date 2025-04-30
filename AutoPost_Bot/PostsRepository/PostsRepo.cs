using AutoPost_Bot.Models;

namespace AutoPost_Bot.PostsRepository
{
    public class PostsRepo : IPostsRepo
    {
        private List<PostModel> posts = [];

        public Task<PostModel?> AddPostAsync(PostModel post)
        {
            try
            {
                posts.Add(post);
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

        public Task<List<PostModel>> GetPostsAsync() => Task.FromResult(posts);

        public Task SavePostChangesAsync(List<PostModel> postsList)
        {
            try
            {
                posts = postsList;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
