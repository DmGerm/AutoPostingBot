using Telegram.Bot;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotService
    {
        public Task<TelegramBotClient> StartBot(string botToken);
        public Task<TelegramBotClient> GetBotClient();
        public Task StopBot();
    }
}
