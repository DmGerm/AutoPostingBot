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

        public List<BotModel>? GetAllBots()
        {
            return _postContext.Bots
           .Include(bot => bot.Groups)
           .ToList() ?? [new BotModel() {
                BotId = Guid.Empty,
                Token = string.Empty,
                IsActive = false,
                Groups = [],
                Posts = []
            }];
        }

        public BotModel? GetBot(Guid botId) =>
            _postContext.Bots.FirstOrDefault(bot => bot.BotId == botId);

        public BotModel CreateNewBot()
        {
            try
            {
                var newBot = new BotModel
                {
                    BotId = Guid.NewGuid(),
                    IsActive = false,
                    Groups = [],
                    Posts = []
                };

                _postContext.Bots.Add(newBot);
                _postContext.SaveChanges();

                return newBot;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating new bot", ex);
            }
        }
    }
}