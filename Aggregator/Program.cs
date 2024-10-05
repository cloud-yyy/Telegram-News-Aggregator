using MessageBroker.Service;
using Publisher.Extensions;
using Reader.Extensions;
using Summarizer.Extensions;
using Aggregator;
using Aggregator.Extensions;
using Reader.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCors();

builder.Services.AddControllers();

builder.ConfigureLogging();

builder.Services.ConfigureContextFactory(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton<Broker>();

builder.Services.AddSingleton<WrappedLinkFactory>();

builder.Services.ConfigureClients();
builder.Services.ConfigureChannels();
builder.Services.ConfigureTopics();
builder.Services.ConfigureSubscribtions();
builder.Services.ConfigureUsers();

builder.Services.ConfigureReader();
builder.Services.ConfigureSummarizer();
builder.Services.ConfigurePublisher();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BrokerConfig>();

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseSwagger();

app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "news");
});

app.ConfigureExceptionHandling();

app.EnsureDatabaseCreated();

app.ConfigureBroker();

app.MapControllers();

app.Run();
