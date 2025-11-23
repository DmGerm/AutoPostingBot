using AutoPost_Bot.Models;

namespace AutoPost_Bot.BotRepo
{
    public interface IBotData
    {
        List<BotModel>? GetAllBots();
        BotModel? GetBot(Guid botId);
        BotModel CreateNewBot();
    }
}

