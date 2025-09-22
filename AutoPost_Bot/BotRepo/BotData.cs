using AutoPost_Bot.Data;

namespace AutoPost_Bot.BotRepo
{
    public class BotData(PostsContext postsContext, IBotService botService) : IBotData
    {
        private readonly PostsContext _postContext = postsContext;
        private readonly IBotService _botService = botService;

        private void UpdateBotStatus(string botToken, bool botStatus)
        {
            _botService.BotStatusChanged += UpdateBotStatusInDatabase;
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
    }
}
