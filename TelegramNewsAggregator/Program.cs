using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using Services.Contracts;
using TelegramNewsAggregator;
using TelegramNewsAggregator.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationContext>
(
	opts =>
	{
		opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
	},
	// Use singletone because when working with threads Dispose called somewhere (System.ObjectDisposedException)
	ServiceLifetime.Singleton
);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ChannelRepository>();
builder.Services.AddScoped<ChannelService>();

builder.Services.AddScoped<SubscribtionsService>();
builder.Services.AddScoped<SubscribtionsController>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UsersController>();

builder.Services.AddSingleton<Services.Contracts.ILogger, ConsoleLogger>();
builder.Services.AddSingleton<MessageBroker>();
builder.Services.AddSingleton<WTelegramClient>();
builder.Services.AddSingleton<ChatGptClient>();
builder.Services.AddSingleton<ApplicationContextFactory>();

builder.Services.ConfigureMessagesReading();
builder.Services.ConfigureSummarizing();

builder.Services.AddScoped<ITelegramChannelIdResolver, WTelegramChannelIdResolver>();
builder.Services.AddScoped<IPublishClient, TelegramBotPublishClient>();

builder.Services.AddScoped<MessageBrokerConfig>();

var app = builder.Build();

app.MapControllers();

app.Run();
