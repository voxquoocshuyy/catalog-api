using Microsoft.EntityFrameworkCore.Design;

namespace ProductCatalog.Infrastructure.Data
{
    public class ProductCatalogDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProductCatalogDbContext>
    {
        public ProductCatalogDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductCatalogDbContext>();
            
            // This is only used for design-time operations (migrations, scaffolding).
            // For development, use environment variable or user secrets.
            // For production, this is never used - connection strings come from configuration.
            var connectionString = Environment.GetEnvironmentVariable("PRODUCTCATALOG_CONNECTIONSTRING") 
                ?? "Host=localhost;Database=productcatalog;Username=postgres;Password=postgres";
            
            optionsBuilder.UseNpgsql(connectionString);

            return new ProductCatalogDbContext(optionsBuilder.Options);
        }
    }
}
