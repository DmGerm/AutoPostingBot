using AutoPost_Bot.Data;
using AutoPost_Bot.Models;

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
            _botService.BotModelUpdateInDb += UpdateBotModelInDb;
        }

        private void UpdateBotModelInDb(BotModel botModel)
        {
            try
            {
                if (_postContext == null)
                    throw new InvalidOperationException("Database context is not available.");

                var existingBot = _postContext.Bots.Find(botModel.BotId);

                if (existingBot == null)
                {
                    _postContext.Bots.Add(botModel);
                }
                else
                {
                    _postContext.Entry(existingBot).CurrentValues.SetValues(botModel);
                }

                _postContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in UpdateBotModelInDb: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
