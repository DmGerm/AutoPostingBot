using Telegram.Bot;

namespace AutoPost_Bot.Models
{
    public class BotModel
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public TelegramBotClient? BotClient { get; set; }
        public bool IsActive { get; set; }
        public Guid PostId { get; set; }
        public IEnumerable<PostModel>? Posts { get; set; }
    }
}
