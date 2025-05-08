using AutoPost_Bot.BotRepo;
using AutoPost_Bot.PostsRepository;
using Telegram.Bot;

namespace AutoPost_Bot.ScheduleService
{
    public class PostSchedulerService(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var postRepo = scope.ServiceProvider.GetRequiredService<IPostsRepo>();
            var botService = scope.ServiceProvider.GetRequiredService<IBotService>();
            var botClient = await botService.GetBotClient();

            var now = DateTime.UtcNow;

            var posts = await postRepo.GetPostsAsync();

            foreach (var post in posts)
            {
                if (now >= post.PostDateTime)
                {
                    await botClient.SendMessage(
                        chatId: post.GroupID,
                        text: post.PostText ?? string.Empty,
                        cancellationToken: stoppingToken
                    );

                    post.PostDateTime = post.PostDateTime
                        .AddDays(post.RepeatDays)
                        .AddHours(post.RepeatHours)
                        .AddMinutes(post.RepeatMinutes);

                    await postRepo.UpdatePostAsync(post);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
