namespace AutoPost_Bot.Models
{
    public class postModel
    {
        public Guid Id { get; set; }
        public string? PostText { get; set; }
        public DateTime? PostDateTime { get; set; }
        public DateTime? RepeatDateTime { get; set; }
    }
}
