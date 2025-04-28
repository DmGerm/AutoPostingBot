using AutoPost_Bot.Handlers;
using AutoPost_Bot.Models;
using AutoPost_Bot.TelegramGroupsRepo;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace AutoPost_Bot.BotRepo
{
    public class BotService(IGroupRepo groupRepo) : IBotService
    {
        private CancellationTokenSource? cts;
        private readonly UpdateHandler updateHandler = new(groupRepo);
        private BotModel bot = new() { Id = new Guid()};

        public Task<TelegramBotClient> GetBotClient()
        {
            if (bot.BotClient == null)
                throw new InvalidOperationException("Bot has not been started yet.");

            return Task.FromResult(bot.BotClient);
        }

        public Task StopBot()
        {
            try
            {
                if (bot.BotClient == null)
                    throw new InvalidOperationException("Bot has not been started yet.");

                cts?.Cancel();

                bot.BotClient = null;
                bot.IsActive = false;

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
                cts = new CancellationTokenSource();

                bot.BotClient = new TelegramBotClient(botToken, cancellationToken: cts.Token);

                bot.Token = botToken;

                var me = await bot.BotClient.GetMe();

                bot.IsActive = true;

                bot.BotClient.OnUpdate += updateHandler.OnUpdate;
                bot.BotClient.OnError += OnError;

                Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

                return bot.BotClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in StartBot: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
        }

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