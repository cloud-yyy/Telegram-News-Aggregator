using TelegramNewsAggregator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<TelegramNewsAggregator.ILogger, ConsoleLogger>();
builder.Services.AddSingleton<MessageBroker>();
builder.Services.AddSingleton<IPublishClient, TelegramBotPublishClient>();
builder.Services.AddSingleton<ISummarizeService, LocalLLMSummarizeService>();
builder.Services.AddSingleton<ITelegramClient, WTelegramClient>();

var app = builder.Build();

var summarizeService = app.Services.GetRequiredService<ISummarizeService>() as IMessageConsumer<MessageDto>;
var publishClient = app.Services.GetRequiredService<IPublishClient>() as IMessageConsumer<SummarizedMessageDto>;
var broker = app.Services.GetRequiredService<MessageBroker>();

broker.Subscribe(summarizeService!);
broker.Subscribe(publishClient!);

app.MapControllers();

app.Run();
