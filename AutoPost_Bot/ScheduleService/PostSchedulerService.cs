using AutoPost_Bot.BotRepo;
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using AutoPost_Bot.TelegramGroupsRepo;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace AutoPost_Bot.ScheduleService
{
    public class PostSchedulerService(IBotService botService, PostsContext postsContext, IGroupRepo groupRepo) : BackgroundService
    {
        private readonly IBotService _botService = botService;
        private readonly PostsContext _postsContext = postsContext
            ?? throw new Exception("Exception in PostScheduler, context can't be null.");
        private readonly IGroupRepo _groupRepo = groupRepo
            ?? throw new Exception("Exception in PostScheduler, groupRepo can't be null.");

        Dictionary<string, BotModel> botModelsDict = [];
        Dictionary<string, TelegramBotClient> activeBots = [];

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            activeBots = _botService.GetActiveBots();

            botModelsDict = _postsContext.Bots
               .Include(b => b.Posts)
               .Where(b => activeBots.ContainsKey(b.Token))
               .ToDictionary(b => b.Token, b => b);

            _botService.BotPostOrStatusChanged += BotDataBaseUpdated_EventHandler;

            while (!stoppingToken.IsCancellationRequested)
            {

                var now = DateTime.UtcNow.AddHours(3);
                var currentDayOfWeek = ConvertDayOfWeek(DateTime.Now.DayOfWeek);

                foreach (var bot in botModelsDict)
                {
                    if (bot.Value.Posts is null || bot.Value.Posts.Count == 0)
                        continue;

                    foreach (var post in bot.Value.Posts)
                        if (now >= post.PostDateTime)
                        {
                            if (post.GroupId != 0 && post.Days.HasFlag(currentDayOfWeek))
                            {
                                try
                                {
                                    await activeBots[bot.Value.Token].SendMessage(
                                                      chatId: post.GroupId,
                                                      text: post.PostText ?? string.Empty,
                                                      cancellationToken: stoppingToken
                                                  );

                                }
                                catch (ApiRequestException ex)
                                {
                                    if (ex.ErrorCode == 404)
                                    {
                                        Console.WriteLine($"Группа с  ID {post.GroupId} не найдена. Удаление группы из БД.");
                                        await groupRepo.RemoveGroupAsync(post.GroupId, bot.Value.Token);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Ошибка отправки сообщения в группу {post.GroupId}: {ex.Message}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            try
                            {
                                if (post.RepeatDays > 0 || post.RepeatHours > 0 || post.RepeatMinutes > 0)
                                {
                                    post.PostDateTime = post.PostDateTime
                                        .AddDays(post.RepeatDays)
                                        .AddHours(post.RepeatHours)
                                        .AddMinutes(post.RepeatMinutes);
                                }
                                else
                                {
                                    post.PostDateTime = DateTime.MaxValue;
                                }
                                bot.Value.Posts.Find(p => p.Id == post.Id)!.PostDateTime = post.PostDateTime;

                                await botService.UpdateBotModel(bot.Value);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                }
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
        }

        private Days ConvertDayOfWeek(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => Days.Monday,
                DayOfWeek.Tuesday => Days.Tuesday,
                DayOfWeek.Wednesday => Days.Wednesday,
                DayOfWeek.Thursday => Days.Thursday,
                DayOfWeek.Friday => Days.Friday,
                DayOfWeek.Saturday => Days.Saturday,
                DayOfWeek.Sunday => Days.Sunday,
                _ => Days.None
            };
        }

        private void BotDataBaseUpdated_EventHandler(object? sender, string e)
        {
            BotModel? updatedBot = postsContext.Bots
            .Include(b => b.Posts)
            .FirstOrDefault(b => b.Token == e);

            if (updatedBot != null)
            {
                botModelsDict[e] = updatedBot;
            }
            activeBots = _botService.GetActiveBots();
        }
    }
}