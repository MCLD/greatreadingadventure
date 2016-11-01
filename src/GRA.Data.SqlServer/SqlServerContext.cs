using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.SqlServer
{
    public class SqlServerContext : Context
    {
        private const string defaultDevCs = @"Server=(localdb)\mssqllocaldb;Database=gra4;Trusted_Connection=True;MultipleActiveResultSets=true";
        public SqlServerContext(IConfigurationRoot config) : base(config) { }
        internal SqlServerContext() : base(defaultDevCs) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(devConnectionString))
            {
                optionsBuilder.UseSqlServer(devConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(config["ConnectionStrings:DefaultConnection"]);
            }
        }
    }
}
