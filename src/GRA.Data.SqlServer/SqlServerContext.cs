using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.SqlServer
{
    public class SqlServerContext : Context
    {
        public SqlServerContext(IConfigurationRoot config) : base(config) { }
        internal SqlServerContext() : base(DefaultConnectionString.SqlServer) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(devConnectionString))
            {
                optionsBuilder.UseSqlServer(devConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(config[ConfigurationKey.DefaultCSSqlServer]);
            }
        }
    }
}
