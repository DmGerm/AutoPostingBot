using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoPost_Bot.BotRepo
{
    public class BotData : IBotData
    {
        private readonly PostsContext _postContext;
        private readonly IBotService _botService;
        public BotData(PostsContext postsContext, IBotService botService)
        {
            _postContext = postsContext ?? throw new ArgumentNullException(nameof(postsContext));
            _botService = botService ?? throw new ArgumentNullException(nameof(botService));
            _botService.BotStatusChanged += UpdateBotStatusInDatabase;

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

        private void UpdateBotStatus(string botToken, bool botStatus)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in UpdateBotStatus: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private void UpdateBotStatusInDatabase(string botToken, bool botStatus) => UpdateBotStatus(botToken, botStatus);
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