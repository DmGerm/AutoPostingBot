using AutoPost_Bot.Data;
using AutoPost_Bot.Handlers;
using AutoPost_Bot.TelegramGroupsRepo;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace AutoPost_Bot.BotRepo
{
    public class BotService : IBotService
    {
        private CancellationTokenSource? cts;
        private readonly UpdateHandler? updateHandler;
        private readonly PostsContext? _postContext;
        private readonly Dictionary<string, (TelegramBotClient Client, CancellationTokenSource Cts)>? _bots;
        public event Action<string, bool>? BotStatusChanged;

        public BotService(IGroupRepo groupRepo, PostsContext postsContext)
        {
            updateHandler = new(groupRepo);
            _postContext = postsContext;

            _bots = _postContext.Bots
                .Where(bot => bot.IsActive)
                .ToDictionary(
                    bot => bot.Token,
                    bot => (new TelegramBotClient(bot.Token), new CancellationTokenSource())
                );

        }

        public async Task<TelegramBotClient> GetBotClient(string botToken)
        {
            if (string.IsNullOrEmpty(botToken))
                throw new InvalidOperationException("Bot token is not provided!");

            if (_postContext is null)
                throw new InvalidOperationException("Database context is not available.");
            try
            {
                var bot = _bots?.GetValueOrDefault(botToken);

                if (bot == null)
                    throw new InvalidOperationException("Bot has not been started yet.");
                return await Task.FromResult(bot.Value.Client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in GetBotClient: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public Task StopBot(string botToken)
        {
            try
            {
                if (string.IsNullOrEmpty(botToken))
                    throw new InvalidOperationException("Bot token is not provided!");

                var bot = _bots?.GetValueOrDefault(botToken);

                if (bot == null)
                    throw new InvalidOperationException("Bot has not been started yet.");

                cts?.Cancel();

                bot.Value.Cts.Cancel();

                BotStatusChanged?.Invoke(botToken, false);

                _bots?.Remove(botToken);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in StopBot: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return Task.CompletedTask;
            }
        }

        public async Task<TelegramBotClient> StartBot(string botToken)
        {
            try
            {
                if (string.IsNullOrEmpty(botToken))
                {
                    throw new InvalidOperationException("Bot token is not provided!");
                }

                if (_bots is null)
                {
                    throw new InvalidOperationException("Bots dictionary is not initialized.");
                }

                if (!_bots.TryAdd(botToken, (new TelegramBotClient(botToken), new CancellationTokenSource())))
                {
                    throw new InvalidOperationException("Bot is already started.");
                }

                if (!_bots.TryGetValue(botToken, out var botValue))
                {
                    throw new InvalidOperationException("Failed to retrieve the bot after adding it.");
                }

                var me = await botValue.Client.GetMe();

                BotStatusChanged?.Invoke(botToken, true);

                if (updateHandler is null)
                {
                    throw new InvalidOperationException("Update handler is not initialized.");
                }

                botValue.Client.OnUpdate += updateHandler.OnUpdate;
                botValue.Client.OnError += OnError;

                Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

                return botValue.Client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in StartBot: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private Task OnError(Exception exception, HandleErrorSource source)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"Error Source: {source}");
                Console.WriteLine($"Exception: {exception.Message}");
                Console.WriteLine(exception.StackTrace);
            });
        }

        //Todo: Переписываем методы работы с ботом по очереди, чтобы можно было работать с несколькими ботами одновременно.
        public bool IsBotActive() => bot.IsActive;

        public string GetBotToken() => bot.Token;

        public void SetBotToken(string botToken)
        {
            try
            {
                bot.Token = botToken;
            }
            catch
            {
                throw new Exception("Error when updating bot Token.");
            }
        }
    }
}