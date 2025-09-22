using Telegram.Bot;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotService
    {
        public Task<TelegramBotClient> StartBot(string botToken);
        public Task<TelegramBotClient> GetBotClient(string botToken);
        public Task StopBot(string botToken);
        public bool IsBotActive();
        public string GetBotToken();
        public void SetBotToken(string botToken);
        public Action<string, bool>? BotStatusChanged { get; set; }

    }
}
