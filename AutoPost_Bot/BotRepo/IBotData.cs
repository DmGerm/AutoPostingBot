namespace AutoPost_Bot.BotRepo
{
    public interface IBotData
    {
        void UpdateBotStatus(string botToken, bool botStatus);
    }
}
