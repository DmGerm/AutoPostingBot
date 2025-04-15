using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AutoPost_Bot.BotRepo
{
    public class BotService : IBotService
    {
        private TelegramBotClient? telegramBotClient;
        private readonly CancellationTokenSource cts = new();

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



                telegramBotClient = new TelegramBotClient(botToken, cancellationToken: cts.Token);

                var me = await telegramBotClient.GetMe();
                telegramBotClient.OnMessage += OnMessage;

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

        async Task OnMessage(Message msg, UpdateType type)
        {
            if (msg.Text is null || telegramBotClient is null) return;
            Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");
            await telegramBotClient.SendMessage(msg.Chat.Id, $"{msg.From?.FirstName} said: {msg.Text}");
        }
    }
}