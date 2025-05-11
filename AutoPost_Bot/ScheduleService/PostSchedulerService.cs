using AutoPost_Bot.BotRepo;
using AutoPost_Bot.PostsRepository;
using AutoPost_Bot.TelegramGroupsRepo;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace AutoPost_Bot.ScheduleService
{
    public class PostSchedulerService(IServiceProvider serviceProvider) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var postRepo = scope.ServiceProvider.GetRequiredService<IPostsRepo>();
                var botService = scope.ServiceProvider.GetRequiredService<IBotService>();
                var groupRepo = scope.ServiceProvider.GetRequiredService<IGroupRepo>();
                ITelegramBotClient botClient;

                try
                {
                    botClient = await botService.GetBotClient();
                }
                catch
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    continue;
                }

                var now = DateTime.Now;
                var posts = await postRepo.GetPostsAsync();

                if (botClient is not null)
                {
                    foreach (var post in posts)
                    {
                        if (now >= post.PostDateTime)
                        {
                            if (post.GroupID != 0 && post.GroupID is not null)
                            {
                                try
                                {
                                    await botClient.SendMessage(
                                        chatId: post.GroupID,
                                        text: post.PostText ?? string.Empty,
                                        cancellationToken: stoppingToken
                                    );

                                }
                                catch (ApiRequestException ex)
                                {
                                    if (ex.ErrorCode == 404)
                                    {
                                        Console.WriteLine($"Группа с ID {post.GroupID} не найдена. Удаление группы из БД.");
                                        await groupRepo.RemoveGroupAsync(post.GroupID.Value);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Ошибка отправки сообщения в группу {post.GroupID}: {ex.Message}");
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

                                await postRepo.UpdatePostAsync(post);
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
        }
    }
}