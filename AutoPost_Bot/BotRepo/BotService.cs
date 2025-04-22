using AutoPost_Bot.Handlers;
using AutoPost_Bot.TelegramGroupsRepo;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace AutoPost_Bot.BotRepo
{
    public class BotService(IGroupRepo groupRepo) : IBotService
    {
        private TelegramBotClient? telegramBotClient;
        private CancellationTokenSource? cts;
        private readonly UpdateHandler updateHandler = new(groupRepo);

        public Task<TelegramBotClient> GetBotClient()
        {
            if (telegramBotClient == null)
                throw new InvalidOperationException("Bot has not been started yet.");

            return Task.FromResult(telegramBotClient);
        }

        public Task StopBot()
        {
            try
            {
                if (telegramBotClient == null)
                    throw new InvalidOperationException("Bot has not been started yet.");

                cts?.Cancel();

                telegramBotClient = null;
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

                telegramBotClient = new TelegramBotClient(botToken, cancellationToken: cts.Token);

                var me = await telegramBotClient.GetMe();
                telegramBotClient.OnUpdate += updateHandler.OnUpdate;
                telegramBotClient.OnError += OnError;

                Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

                return telegramBotClient;
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
    }
}