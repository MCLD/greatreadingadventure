using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.SQLite
{
    public class SQLiteContext : Context
    {
        public SQLiteContext(IConfiguration config) : base(config) { }
        internal SQLiteContext() : base(DefaultConnectionString.SQLite) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(devConnectionString))
            {
                optionsBuilder.UseSqlite(devConnectionString);
            }
            else
            {
                optionsBuilder.UseSqlite(_config[ConfigurationKey.DefaultCSSQLite]);
            }
        }
    }
}
