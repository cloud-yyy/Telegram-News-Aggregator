using TelegramNewsAggregator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<TelegramNewsAggregator.ILogger, ConsoleLogger>();
builder.Services.AddSingleton<IMessageSerializer, MessageSerializer>();
builder.Services.AddSingleton<ITelegramClient, WTelegramClient>();

var app = builder.Build();

app.MapControllers();

app.Run();
