using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CompanyManagement.Data.Data;

namespace CompanyManagement.Data.Configurations
{
    public static class DatabaseConfiguration
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
          var connectionString = configuration.GetConnectionString("DefaultConnection");

          services.AddDbContext<CompanyDbContext>(options =>
              options.UseNpgsql(connectionString)); 
        }
    }
}
