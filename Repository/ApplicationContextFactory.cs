using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repository
{
    public class ApplicationContextFactory
    {
        private readonly string _connectionString;

        public ApplicationContextFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public ApplicationContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseNpgsql(_connectionString)
                .Options;

            return new ApplicationContext(options);
        }
    }
}