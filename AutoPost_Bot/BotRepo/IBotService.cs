using Telegram.Bot;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotService
    {
        public Task<TelegramBotClient> StartBot(string botToken);
        public Task<List<TelegramBotClient?>> GetBotClients();
        public Task StopBot();
        public bool IsBotActive();
        public string GetBotToken();
        public void SetBotToken(string botToken);
    }
}
