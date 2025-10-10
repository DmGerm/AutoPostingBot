namespace AutoPost_Bot.BotRepo
{
    public interface IBotData
    {
        List<string>? GetAllBots();
        public List<string> GetAllBotTokensFromDb();
    }
}
