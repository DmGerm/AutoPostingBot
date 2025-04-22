using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoPost_Bot.BotRepo;
using AutoPost_Bot.Components;
using AutoPost_Bot.PostsRepository;
using AutoPost_Bot.TelegramGroupsRepo;

namespace AutoPost_Bot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                        .ConfigureContainer<ContainerBuilder>(cb =>
                        {
                            cb.RegisterType<BotService>()
                                .As<IBotService>()
                                .SingleInstance();

                            cb.RegisterType<PostsRepo>()
                            .As<IPostsRepo>().SingleInstance();

                            cb.RegisterType<GroupRepo>()
                            .As<IGroupRepo>().SingleInstance();

                        });

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Console.WriteLine($"🔥 Unobserved task exception: {e.Exception.Message}");
                e.SetObserved();
            };

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
