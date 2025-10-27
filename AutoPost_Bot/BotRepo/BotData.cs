using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.BotRepo
{
    public class BotData : IBotData
    {
        private readonly PostsContext _postContext;
        private readonly IBotService _botService;
        public BotData(PostsContext postsContext)
        {
            _postContext = postsContext ?? throw new ArgumentNullException(nameof(postsContext));

            if (_postContext.Bots == null)
            {
                throw new InvalidOperationException("Bots DbSet is not initialized.");
            }

            if (!_postContext.Bots.Any())
            {
                _postContext.Bots.Add(new Models.BotModel
                {
                    Token = string.Empty,
                    IsActive = false
                });
                _postContext.SaveChanges();
            }
        }

        public List<string> GetAllBotTokensFromDb() => _postContext.Bots.Select(b => b.Token).ToList();

        public List<BotModel>? GetAllBots() => _postContext.Bots
            .Include(bot => bot.Groups)
            .ToList();

        public Task UpdateBotModel(BotModel model)
        {
            try
            {
                _postContext.Bots.Update(model);
                return _postContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in UpdateBotModel: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}