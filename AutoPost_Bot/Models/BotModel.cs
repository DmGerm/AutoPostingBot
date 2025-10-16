namespace AutoPost_Bot.Models
{
    public class BotModel
    {
        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public virtual IEnumerable<GroupModel>? Groups { get; set; }
        public virtual List<PostModel>? Posts { get; set; }
    }
}
