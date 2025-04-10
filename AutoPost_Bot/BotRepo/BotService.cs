using System.Text.RegularExpressions;
using Telegram.Bot;

namespace AutoPost_Bot.BotRepo
{
    public class BotService : IBotService
    {

        private TelegramBotClient? telegramBotClient;
        private CancellationTokenSource? cancellationTokenSource;

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
                cancellationTokenSource?.Cancel();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in StopTheBot: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                return Task.FromResult(false);
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

                if (string.IsNullOrEmpty(botToken))
                {
                    botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? string.Empty;

                    if (string.IsNullOrEmpty(botToken))
                    {
                        var stringFromEnv = await File.ReadAllTextAsync("token.env");
                        string pattern = @"^(?:\w+)=([\w:]+)$";

                        if (!Regex.IsMatch(stringFromEnv, pattern))
                        {
                            throw new InvalidOperationException("Invalid token.env file format.");
                        }

                        botToken = Regex.Match(stringFromEnv, pattern).Groups[1].Value;
                    }


                }

                telegramBotClient = new TelegramBotClient(botToken, cancellationToken: cancellationTokenSource.Token);

                var me = await telegramBotClient.GetMe();
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

    }
}
