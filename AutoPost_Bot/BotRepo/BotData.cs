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
        }

        public List<string> GetAllBotTokensFromDb() => _postContext.Bots.Select(b => b.Token).ToList();

        public List<BotModel>? GetAllBots() => _postContext.Bots
            .Include(bot => bot.Groups)
            .ToList();

        public BotModel? GetBot(Guid botId) =>
            _postContext.Bots.FirstOrDefault(bot => bot.BotId == botId);
    }
}