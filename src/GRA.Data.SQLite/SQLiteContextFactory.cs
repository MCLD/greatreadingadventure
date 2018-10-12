using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GRA.Data.SQLite
{
    public class SQLiteContextFactory : IDesignTimeDbContextFactory<SQLiteContext>
    {
        public SQLiteContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SQLiteContext>();
            optionsBuilder.UseSqlite(Development.SQLiteCS);
            return new SQLiteContext(optionsBuilder.Options);
        }
    }
}
