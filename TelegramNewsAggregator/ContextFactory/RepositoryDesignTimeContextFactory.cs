using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace TelegramNewsAggregator;

public class RepositoryDesignTimeContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
	public ApplicationContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.Build();

		var builder = new DbContextOptionsBuilder<ApplicationContext>()
			.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
				b => b.MigrationsAssembly("TelegramNewsAggregator"));

		return new ApplicationContext(builder.Options);
	}
}
