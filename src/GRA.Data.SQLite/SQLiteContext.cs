using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.SQLite
{
    public class SQLiteContext : Context
    {
        public SQLiteContext(IConfigurationRoot config) : base(config) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(config["ConnectionStrings:DefaultConnection"]);
        }
    }
}
