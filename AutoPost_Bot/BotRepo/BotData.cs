using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.BotRepo
{
    public class BotData : IBotData
    {
        private readonly PostsContext _postContext;
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
                    BotId = Guid.NewGuid(),
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

        public BotModel? GetBot(Guid botId) =>
            _postContext.Bots.FirstOrDefault(bot => bot.BotId == botId);
    }
}