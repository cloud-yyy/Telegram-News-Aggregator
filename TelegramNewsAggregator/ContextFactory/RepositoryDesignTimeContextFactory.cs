using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace TelegramNewsAggregator;

public class RepositoryDesignTimeContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
	public RepositoryContext CreateDbContext(string[] args)
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json")
			.Build();

		var builder = new DbContextOptionsBuilder<RepositoryContext>()
			.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
				b => b.MigrationsAssembly("TelegramNewsAggregator"));

		return new RepositoryContext(builder.Options);
	}
}
