using AutoPost_Bot.BotRepo;
using AutoPost_Bot.Data;
using AutoPost_Bot.Models;
using AutoPost_Bot.TelegramGroupsRepo;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace AutoPost_Bot.ScheduleService
{
    public class PostSchedulerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBotService _botService;
        private readonly IGroupRepo _groupRepo;

        private Dictionary<string, BotModel> _botModels = [];
        private Dictionary<string, TelegramBotClient> _activeBots = [];

        public PostSchedulerService(
            IServiceScopeFactory scopeFactory,
            IBotService botService,
            IGroupRepo groupRepo)
        {
            _scopeFactory = scopeFactory;
            _botService = botService;
            _groupRepo = groupRepo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PostsContext>();

                _activeBots = _botService.GetActiveBots();
                var activeBotTokens = _activeBots.Keys.ToList();

                _botModels = await db.Bots
                    .Include(b => b.Posts)
                    .Where(b => activeBotTokens.Contains(b.Token))
                    .ToDictionaryAsync(b => b.Token, b => b, cancellationToken: stoppingToken);
            }

            _botService.BotPostOrStatusChanged += OnBotDataChanged;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndSendPosts(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Scheduler] Ошибка: {ex.Message}");
                }
            }
        }

        private async Task CheckAndSendPosts(CancellationToken token)
        {
            var now = DateTime.UtcNow.AddHours(3);
            var currentDay = ConvertDayOfWeek(DateTime.Now.DayOfWeek);

            foreach (var (tokenKey, bot) in _botModels)
            {
                if (bot.Posts is null || bot.Posts.Count == 0)
                    continue;

                foreach (var post in bot.Posts.Where(p => now >= p.PostDateTime && p.Days.HasFlag(currentDay)))
                {
                    try
                    {
                        await _activeBots[tokenKey].SendMessage(
                            chatId: post.GroupId,
                            text: post.PostText ?? string.Empty,
                            cancellationToken: token
                        );
                    }
                    catch (ApiRequestException ex) when (ex.ErrorCode == 404)
                    {
                        Console.WriteLine($"Группа {post.GroupId} не найдена. Удаляем из БД.");
                        await _groupRepo.RemoveGroupAsync(post.GroupId, bot.Token);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при отправке: {ex.Message}");
                    }

                    await UpdatePostSchedule(bot, post);
                }
            }
        }

        private async Task UpdatePostSchedule(BotModel bot, PostModel post)
        {
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

                bot.Posts.Find(p => p.Id == post.Id)!.PostDateTime = post.PostDateTime;
                await _botService.UpdateBotModel(bot);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении поста: {ex.Message}");
            }
        }

        private Days ConvertDayOfWeek(DayOfWeek dayOfWeek) => dayOfWeek switch
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

        private void OnBotDataChanged(object? sender, string token)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PostsContext>();

            var updatedBot = db.Bots
                .Include(b => b.Posts)
                .FirstOrDefault(b => b.Token == token);

            if (updatedBot != null)
                _botModels[token] = updatedBot;

            _activeBots = _botService.GetActiveBots();
        }
    }
}
