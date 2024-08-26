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
		opts
			.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
			.EnableSensitiveDataLogging();
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

builder.Services.AddSingleton<MessageBroker>();
builder.Services.AddSingleton<WTelegramClient>();
builder.Services.AddSingleton<ChatGptClient>();
builder.Services.AddSingleton<ApplicationContextFactory>();

builder.Services.ConfigureMessagesReading();
builder.Services.ConfigureSummarizing();

builder.Services.AddScoped<ITelegramChannelIdResolver, WTelegramChannelIdResolver>();
builder.Services.AddScoped<IPublishClient, TelegramBotPublishClient>();

builder.Services.AddScoped<MessageBrokerConfig>();
builder.Services.AddScoped<ApplicationEntryPoint>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<ApplicationEntryPoint>>();
app.ConfigureExceptionHandler(logger);

if (app.Environment.IsProduction())
	app.UseHsts();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var entryPoint = scope.ServiceProvider.GetRequiredService<ApplicationEntryPoint>();
	await entryPoint.Entry();
}

app.Run();
