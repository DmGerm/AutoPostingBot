namespace AutoPost_Bot.Models
{
    public class BotModel
    {
        public Guid BotId { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? BotName { get; set; }

        public virtual IEnumerable<GroupModel>? Groups { get; set; }
        public virtual List<PostModel>? Posts { get; set; }
    }
}
