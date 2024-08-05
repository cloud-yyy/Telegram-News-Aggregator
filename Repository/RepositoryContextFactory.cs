using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repository
{
    public class RepositoryContextFactory
    {
        private readonly string _connectionString;

        public RepositoryContextFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public RepositoryContext Create()
        {
            var options = new DbContextOptionsBuilder<RepositoryContext>()
                .UseNpgsql(_connectionString)
                .Options;

            return new RepositoryContext(options);
        }
    }
}