using Microsoft.EntityFrameworkCore;
using Repository;
using Services.Channels;
using Services.Subscribtions;
using Services.Users;
using Shared.Clients;
using Shared.Dtos;
using Aggregator.Controllers;
using Services.Topics;

namespace Aggregator.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
        }

        public static void ConfigureContextFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextFactory<ApplicationContext>(opts =>
            {
                opts
                    .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                    .EnableSensitiveDataLogging();
            });
        }

        public static void ConfigureClients(this IServiceCollection services)
        {
            services.AddSingleton<WTelegramClient>((provider) =>
            {
                var user = new MessageReaderUserDto
                (
                    Environment.GetEnvironmentVariable("api_id")!,
                    Environment.GetEnvironmentVariable("api_hash")!,
                    Environment.GetEnvironmentVariable("phone_number")!,
                    () =>
                    {
                        Console.WriteLine("Code: ");
                        return Console.ReadLine()!;
                    }
                );

                return new WTelegramClient(provider.GetRequiredService<ILogger<WTelegramClient>>(), user);
            });

            services.AddSingleton<ChatGptClient>();
        }

        public static void ConfigureChannels(this IServiceCollection services)
        {
            services.AddScoped<ChannelRepository>();
            services.AddScoped<ChannelService>();
            services.AddScoped<ITelegramChannelIdResolver, WTelegramChannelIdResolver>();
        }

        public static void ConfigureTopics(this IServiceCollection services)
        {
            services.AddScoped<TopicsService>();
            services.AddScoped<TopicsController>();
        }

        public static void ConfigureSubscribtions(this IServiceCollection services)
        {
            services.AddScoped<SubscribtionsService>();
            services.AddScoped<SubscribtionsController>();
        }

        public static void ConfigureUsers(this IServiceCollection services)
        {
            services.AddScoped<UserService>();
            services.AddScoped<UsersController>();
        }
    }
}
