using Microsoft.EntityFrameworkCore;
using Repository;
using TelegramNewsAggregator;
using TelegramNewsAggregator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<RepositoryContext>
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
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<MessagesTagsRepository>();

builder.Services.AddScoped<ChannelService>();

builder.Services.AddSingleton<TelegramNewsAggregator.ILogger, ConsoleLogger>();
builder.Services.AddSingleton<MessageBroker>();
builder.Services.AddSingleton<WTelegramClient>();
builder.Services.AddSingleton<RepositoryContextFactory>();

builder.Services.AddScoped<ITelegramMessageReader, WTelegramMessageReader>();
builder.Services.AddScoped<ITelegramChannelIdResolver, WTelegramChannelIdResolver>();
builder.Services.AddScoped<IPublishClient, TelegramBotPublishClient>();
builder.Services.AddScoped<ISummarizeService, SummarizeServiceMock>();
builder.Services.AddScoped<ITagsExtractService, RakeTagsExtractService>();

builder.Services.AddScoped<MessageDbWriter>();
builder.Services.AddScoped<TagsDbWriter>();

builder.Services.AddScoped<MessageBrokerConfig>();

var app = builder.Build();

app.MapControllers();

app.Run();
