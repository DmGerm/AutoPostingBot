namespace AutoPost_Bot.Models
{
    public class GroupModel
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual IEnumerable<BotModel>? Bots { get; set; }
        public virtual IEnumerable<PostModel>? Posts { get; set; }
    }
}
