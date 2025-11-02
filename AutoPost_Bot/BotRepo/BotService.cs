using AutoPost_Bot.Data;
using AutoPost_Bot.Handlers;
using AutoPost_Bot.Models;
using AutoPost_Bot.TelegramGroupsRepo;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace AutoPost_Bot.BotRepo
{
    public class BotService : IBotService
    {
        private readonly PostsContext _postContext;
        private readonly IGroupRepo _groupRepo;
        private readonly IBotData? _botData;

        private readonly ConcurrentDictionary<string, (TelegramBotClient Client, CancellationTokenSource Cts, UpdateHandler Handler)> _activeBots = new();

        public event Action<string, bool>? BotStatusChanged;
        public event EventHandler<string>? BotPostOrStatusChanged;

        public BotService(IGroupRepo groupRepo, PostsContext postsContext, IBotData? botData)
        {
            _groupRepo = groupRepo ?? throw new InvalidOperationException("Group repository is not available.");
            _postContext = postsContext ?? throw new InvalidOperationException("Database context is not available.");
            _botData = botData;
        }

        public async Task<TelegramBotClient> GetBotClient(string botToken)
        {
            if (string.IsNullOrEmpty(botToken))
                throw new InvalidOperationException("Bot token is not provided!");

            if (!_activeBots.TryGetValue(botToken, out var bot))
                throw new InvalidOperationException("Bot has not been started yet.");

            return await Task.FromResult(bot.Client);
        }

        public async Task<TelegramBotClient> StartBot(string botToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(botToken))
                    throw new InvalidOperationException("Bot token is not provided!");

                if (_activeBots.ContainsKey(botToken))
                    throw new InvalidOperationException("Bot is already started.");

                var cts = new CancellationTokenSource();
                var client = new TelegramBotClient(botToken, cancellationToken: cts.Token);
                var handler = new UpdateHandler(_groupRepo, botToken);

                await client.DeleteWebhook();

                var me = await client.GetMe();

                if (!_activeBots.TryAdd(botToken, (client, cts, handler)))
                    throw new InvalidOperationException("Failed to register bot in active list.");

                client.OnUpdate += handler.OnUpdate;
                client.OnError += OnError;

                BotStatusChanged?.Invoke(botToken, true);
                BotPostOrStatusChanged?.Invoke(this, botToken);

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

                bot.Cts.Cancel();

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
            _botData?.GetAllBots() ?? [];

        public async Task UpdateBotModel(BotModel model)
        {
            try
            {
                if (_botData is null)
                    throw new InvalidOperationException("Bot data service is not available.");

                BotPostOrStatusChanged?.Invoke(this, model.Token);
                await _botData.UpdateBotModel(model);
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
