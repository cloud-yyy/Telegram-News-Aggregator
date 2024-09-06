using MessageBroker.Service;
using Publisher.Extensions;
using Reader.Extensions;
using Summarizer.Extensions;
using Aggregator;
using Aggregator.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.ConfigureLogging();

builder.Services.ConfigureContextFactory(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton<Broker>();

builder.Services.ConfigureClients();
builder.Services.ConfigureChannels();
builder.Services.ConfigureSubscribtions();
builder.Services.ConfigureUsers();

builder.Services.ConfigureReader();
builder.Services.ConfigureSummarizer();
builder.Services.ConfigurePublisher();

builder.Services.AddSingleton<BrokerConfig>();

var app = builder.Build();

app.ConfigureExceptionHandling();

app.EnsureDatabaseCreated();

app.ConfigureBroker();

app.MapControllers();

app.Run();
