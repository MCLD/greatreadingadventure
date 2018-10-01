using Microsoft.EntityFrameworkCore;

namespace GRA.Data.SQLite
{
    public class SQLiteContext : Context
    {
        public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options) { }
    }
}
