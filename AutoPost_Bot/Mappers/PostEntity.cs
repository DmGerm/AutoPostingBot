namespace AutoPost_Bot.Mappers
{
    public class PostEntity
    {
        public string? PostText { get; set; }
        public long GroupID { get; set; }
        public DateTime PostDateTime { get; set; }
        public int RepeatDays { get; set; }
        public int RepeatHours { get; set; }
        public int RepeatMinutes { get; set; }
    }
}
