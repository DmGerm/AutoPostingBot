using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoPost_Bot.BotRepo;
using AutoPost_Bot.Components;
using AutoPost_Bot.Data;
using AutoPost_Bot.PostsRepository;
using AutoPost_Bot.TelegramGroupsRepo;
using Microsoft.EntityFrameworkCore;

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

                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "data"));
                            var dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
                            var postsFilePath = Path.Combine(dataDir, "posts.db");
                            var postConnectionString = $"Data Source={postsFilePath};Cache=Shared;Mode=ReadWriteCreate;Foreign Keys=True;Pooling=True";

                            cb.Register(c =>
                            {
                                var optionBuilder = new DbContextOptionsBuilder<PostsContext>();
                                optionBuilder.UseSqlite(postConnectionString)
                                             .UseLazyLoadingProxies();
                                return new PostsContext(optionBuilder.Options);
                            }).InstancePerDependency();
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
