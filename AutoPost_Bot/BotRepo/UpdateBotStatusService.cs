using AutoPost_Bot.Data;

namespace AutoPost_Bot.BotRepo
{
    public class UpdateBotStatusService
    {
        private readonly IBotService _botService;
        private readonly PostsContext? _postContext;
        public UpdateBotStatusService(IBotService botService, PostsContext? postContext)
        {
            _botService = botService;
            _postContext = postContext;
            _botService.BotStatusChanged += UpdateBotStatusInDatabase;
        }

        private void UpdateBotStatus(Guid BotId, bool botStatus)
        {
            try
            {
                if (_postContext is null)
                    throw new InvalidOperationException("Database context is not available.");

                var bot = _postContext.Bots.FirstOrDefault(b => b.Token == botToken);
                if (bot != null)
                {
                    bot.IsActive = botStatus;
                }
                else
                {
                    _postContext.Bots.Add(new Models.BotModel
                    {
                        Token = botToken,
                        IsActive = botStatus
                    });
                }

                _postContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in UpdateBotStatus: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private void UpdateBotStatusInDatabase(string botToken, bool botStatus) => UpdateBotStatus(botToken, botStatus);
    }
}
