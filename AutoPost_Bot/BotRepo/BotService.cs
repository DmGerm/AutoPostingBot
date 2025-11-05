using AutoPost_Bot.Data;
using AutoPost_Bot.Handlers;
using AutoPost_Bot.Models;
using AutoPost_Bot.TelegramGroupsRepo;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace AutoPost_Bot.BotRepo
{
    public class BotService(IGroupRepo groupRepo, PostsContext postsContext, IBotData? botData)
        : IBotService
    {
        private readonly PostsContext _postContext = postsContext 
                                                     ?? throw new InvalidOperationException("Database context is not available.");
        private readonly IGroupRepo _groupRepo = groupRepo 
                                                 ?? throw new InvalidOperationException("Group repository is not available.");

        private readonly ConcurrentDictionary<Guid, (TelegramBotClient Client, CancellationTokenSource Cts, UpdateHandler Handler)> _activeBots = new();

        public event Action<Guid, bool>? BotStatusChanged;
        public event EventHandler<Guid>? BotPostOrStatusChanged;

        public async Task<TelegramBotClient> GetBotClient(string botToken)
        {
            if (string.IsNullOrEmpty(botToken))
                throw new InvalidOperationException("Bot token is not provided!");

            if (!_activeBots.TryGetValue(botToken, out var bot))
                throw new InvalidOperationException("Bot has not been started yet.");

            return await Task.FromResult(bot.Client);
        }

        public async Task<TelegramBotClient> StartBot(Guid botId, string botToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(botToken))
                    throw new InvalidOperationException("Bot token is not provided!");

                if (_activeBots.ContainsKey(botId))
                    throw new InvalidOperationException("Bot is already started.");

                var cts = new CancellationTokenSource();
                var client = new TelegramBotClient(botToken, cancellationToken: cts.Token);
                var handler = new UpdateHandler(_groupRepo, botToken);

                await client.DeleteWebhook(cancellationToken: cts.Token);

                var me = await client.GetMe(cancellationToken: cts.Token);

                if (!_activeBots.TryAdd(botId, (client, cts, handler)))
                    throw new InvalidOperationException("Failed to register bot in active list.");

                client.OnUpdate += handler.OnUpdate;
                client.OnError += OnError;

                BotStatusChanged?.Invoke(botId, true);
                BotPostOrStatusChanged?.Invoke(this, botId);

                Console.WriteLine($"✅ @{me.Username} is running...");
                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in StartBot: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public async Task StopBot(string botToken)
        {
            try
            {
                if (string.IsNullOrEmpty(botToken))
                    throw new InvalidOperationException("Bot token is not provided!");

                if (!_activeBots.TryRemove(botToken, out var bot))
                    throw new InvalidOperationException("Bot has not been started yet.");

                await bot.Cts.CancelAsync();

                bot.Client.OnUpdate -= bot.Handler.OnUpdate;
                bot.Client.OnError -= OnError;

                BotStatusChanged?.Invoke(botToken, false);
                BotPostOrStatusChanged?.Invoke(this, botToken);

                Console.WriteLine($"🛑 Bot with token ending {botToken[^6..]} stopped.");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in StopBot: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public bool IsBotActive(string botToken) =>
            _activeBots.ContainsKey(botToken);

        public Dictionary<string, TelegramBotClient> GetActiveBots() =>
            _activeBots.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Client);

        public List<BotModel> GetBotModels() =>
            botData?.GetAllBots() ?? [];

        public async Task UpdateBotModel(BotModel model)
        {
            try
            {
                if (botData is null)
                    throw new InvalidOperationException("Bot data service is not available.");

                BotPostOrStatusChanged?.Invoke(this, model.Token);
                await botData.UpdateBotModel(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in UpdateBotModel: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private Task OnError(Exception exception, HandleErrorSource source)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"⚠️ Telegram Error Source: {source}");
                Console.WriteLine($"Exception: {exception.Message}");
                Console.WriteLine(exception.StackTrace);
            });
        }
    }
}
