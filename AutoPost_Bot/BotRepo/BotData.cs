using AutoPost_Bot.Data;

namespace AutoPost_Bot.BotRepo
{
    public class BotData : IBotData
    {
        private readonly PostsContext _postContext;
        private readonly IBotService _botService;
        public BotData(PostsContext postsContext, IBotService botService)
        {
            _postContext = postsContext;
            _botService = botService;
            _botService.BotStatusChanged += UpdateBotStatusInDatabase;
        }

        private void UpdateBotStatus(string botToken, bool botStatus)
        {
            if (_postContext is null)
                throw new InvalidOperationException("Database context is not available.");

            var bot = _postContext.Bots.FirstOrDefault(b => b.Token == botToken);
            if (bot != null)
            {
                bot.IsActive = botStatus;
                _postContext.SaveChanges();
            }
            else
            {
                _postContext.Bots.Add(new Models.BotModel
                {
                    Token = botToken,
                    IsActive = botStatus
                });
                _postContext.SaveChanges();
            }
        }

        private void UpdateBotStatusInDatabase(string botToken, bool botStatus) => UpdateBotStatus(botToken, botStatus);
        public List<string> GetAllBotTokensFromDb() =>  _postContext.Bots.Select(b => b.Token).ToList();
    }
}
