using Microsoft.Extensions.Configuration;

namespace CompanyManagement.Data.Configurations
{
    public class DataSettings
    {
        public string ConnectionString { get; }

        public DataSettings(IConfiguration configuration)
        {
          ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }
    }
}
