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
                // .UseRowNumberForPaging is for SQL Server 2008 compatibility:
                // https://github.com/aspnet/EntityFramework/issues/4616
                optionsBuilder.UseSqlServer(config[ConfigurationKey.DefaultCSSqlServer],
                    _ => _.UseRowNumberForPaging());
            }
        }
    }
}
