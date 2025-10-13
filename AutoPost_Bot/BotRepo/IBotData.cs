using AutoPost_Bot.Models;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotData
    {
        List<string>? GetAllBots();
        public List<BotModel> GetAllBotTokensFromDb();
    }
}
