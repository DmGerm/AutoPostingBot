namespace AutoPost_Bot.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? PostText { get; set; }
        public DateTime PostDateTime { get; set; }
        public Days Days { get; set; }
        public int RepeatDays { get; set; }
        public int RepeatHours { get; set; }
        public int RepeatMinutes { get; set; }
        public virtual BotModel? Bot { get; set; }
        public string BotToken { get; set; } = string.Empty;
        public long GroupId { get; set; }
        public virtual GroupModel? Group { get; set; }
    }
}
