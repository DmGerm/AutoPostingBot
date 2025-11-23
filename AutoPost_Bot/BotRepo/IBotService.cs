using AutoPost_Bot.Models;
using Telegram.Bot;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotService
    {
        public Task<TelegramBotClient> StartBot(Guid botId, string botToken);
        public Task<TelegramBotClient> GetBotClient(Guid botId);
        public Task StopBot(Guid botId);
        public bool IsBotActive(Guid botId);
        public event Action<BotModel>? BotModelUpdateInDb;
        public event EventHandler<BotModel>? BotPostOrStatusChanged;
        public List<BotModel> GetBotModels();
        public Task UpdateBotModel(BotModel model);
        public Dictionary<Guid, TelegramBotClient> GetActiveBots();
        public BotModel CreateNewBot();
    }
}
