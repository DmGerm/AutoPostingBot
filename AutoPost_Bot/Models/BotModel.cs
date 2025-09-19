namespace AutoPost_Bot.Models
{
    public class BotModel
    {
        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public IEnumerable<PostModel>? Posts { get; set; }
    }
}
