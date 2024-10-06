using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository;

namespace Publisher.Utils
{
    public class WrappedLinkFactory
    {
        private readonly IDbContextFactory<ApplicationContext> _contextFactory;
        private readonly string _domainName;

        public WrappedLinkFactory(
            IDbContextFactory<ApplicationContext> contextFactory,
            IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _domainName = configuration.GetValue<string>("DomainName")!;
        }

        public async Task<string> CreateWrappedLink(string innerLink)
        {
            using var context = _contextFactory.CreateDbContext();

            var link = new WrappedLink()
            {
                Id = Guid.NewGuid(),
                InnerLink = innerLink
            };

            context.WrappedLinks.Add(link);
            await context.SaveChangesAsync();

            return $"{_domainName}/{link.Id}";
        }
    }
}
