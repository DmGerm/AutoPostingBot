using AutoPost_Bot.Handlers;
using AutoPost_Bot.Models;
using AutoPost_Bot.TelegramGroupsRepo;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace AutoPost_Bot.BotRepo
{
    //Todo: Переделвыем структуру хранения данных о ботах на БД - начать с сущностей, далее все манипуляции данный класс будет проводить с базой, а не со словарем.
    public class BotService(IGroupRepo groupRepo) : IBotService
    {
        private readonly UpdateHandler _updateHandler = new(groupRepo);
        private readonly Dictionary<Guid, Tuple<BotModel, CancellationTokenSource?>> _botModels = [];

        async Task<List<TelegramBotClient?>> IBotService.GetBotClients()
        {
            if (_botModels is { Count: 0 })
                throw new InvalidOperationException("Bot has not been started yet.");

            return await Task.FromResult(_botModels.Values.Select(botModel => botModel.Item1.BotClient).ToList());
        }

        public Task StopBot(Guid botId)
        {
            try
            {
                if (_botModels is { Count: 0 })
                    throw new InvalidOperationException("Bot has not been started yet.");

                var botEntry = _botModels.FirstOrDefault(entry => entry.Value.Item1.Id.Equals(botId));

                botEntry.Value.Item2?.Cancel();

                botEntry.Value.Item1.BotClient = null;
                botEntry.Value.Item1.IsActive = false;

                _botModels.Remove(botEntry.Key);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in StopBot: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
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

                var botId = Guid.NewGuid();
                var tokenSource = new CancellationTokenSource();

                _botModels.Add(botId, new Tuple<BotModel, CancellationTokenSource?>(new BotModel()
                {
                    Id = botId, BotClient = new TelegramBotClient(botToken, new HttpClient(), tokenSource.Token)
                }, tokenSource));

               if(!_botModels.TryGetValue(botId, out var botModel))
               {
                   throw new InvalidOperationException("Bot with this id has not been started yet.");
               }
               botModel.Item1.IsActive = true;

                var me = await (botModel.Item1.BotClient
                                ?? throw new InvalidOperationException()).GetMe(cancellationToken: tokenSource.Token);

                botModel.Item1.BotClient.OnUpdate += _updateHandler.OnUpdate;
                botModel.Item1.BotClient.OnError += OnError;

                Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");

                return botModel.Item1.BotClient;
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
        //ToDo: Переделываем на Tuple, двигаемся дальше
        public bool IsBotActive() => _botModels.IsActive;

        public string GetBotToken() => _botModels.Token;

        public void SetBotToken(string botToken)
        {
            try
            {
                _botModels.Token = botToken;
            }
            catch
            {
                throw new Exception("Error when updating bot Token.");
            }
        }
    }
}