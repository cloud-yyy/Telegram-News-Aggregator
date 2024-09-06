using Entities.ErrorModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Aggregator.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void EnsureDatabaseCreated(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            using (var context = scope.ServiceProvider
                .GetRequiredService<IDbContextFactory<ApplicationContext>>().CreateDbContext())
            {
                context.Database.EnsureCreated();
            }
        }

        public static void ConfigureBroker(this WebApplication app)
        {
            var config = app.Services.GetRequiredService<BrokerConfig>();
            config.Configure();
        }

        public static void ConfigureExceptionHandling(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    logger.LogError(exceptionHandlerPathFeature?.Error.ToString());

                    var detail = exceptionHandlerPathFeature?.Error.Message ?? "An exception was thrown during the request.";
                    var traceId = context.TraceIdentifier;

                    var errorDetails = new ErrorDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                        Title = "An unexpected error occurred.",
                        Status = context.Response.StatusCode,
                        Detail = detail,
                        TraceId = traceId
                    };

                    await context.Response.WriteAsync(errorDetails.ToString());
                });
            });
        }
    }
}