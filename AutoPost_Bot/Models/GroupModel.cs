namespace AutoPost_Bot.Models
{
    public class GroupModel
    {
        public long GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual IEnumerable<BotModel>? Bots { get; set; }
    }
}
