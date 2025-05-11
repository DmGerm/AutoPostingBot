namespace AutoPost_Bot.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? PostText { get; set; }
        public long? GroupID { get; set; }
        public string? BotID { get; set; }
        public DateTime PostDateTime { get; set; }
        public int RepeatDays { get; set; }
        public int RepeatHours { get; set; }
        public int RepeatMinutes { get; set; }
        public virtual GroupModel? Group { get; set; }
    }
}
