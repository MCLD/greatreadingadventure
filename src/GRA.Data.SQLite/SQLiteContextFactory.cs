using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GRA.Data.SQLite
{
    internal class SQLiteContextFactory : IDbContextFactory<SQLiteContext>
    {
        SQLiteContext IDbContextFactory<SQLiteContext>.Create(DbContextFactoryOptions options)
        {
            return new SQLiteContext();
        }
    }
}
