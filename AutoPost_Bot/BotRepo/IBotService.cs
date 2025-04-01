using Telegram.Bot;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotService
    {
        public Task<TelegramBotClient> StartBot(CancellationTokenSource cancellationTokenSource);
        public Task<TelegramBotClient> GetBotClient();
    }
}
