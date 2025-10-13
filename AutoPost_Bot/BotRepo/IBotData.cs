using AutoPost_Bot.Models;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotData
    {
        List<BotModel>? GetAllBots();
        public List<BotModel> GetAllBotTokensFromDb();
    }
}
