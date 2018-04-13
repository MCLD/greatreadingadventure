
namespace GRA
{
    public static class DefaultConnectionString
    {
        public const string SqlServer = @"Server=(localdb)\mssqllocaldb;Database=gra;Trusted_Connection=True;MultipleActiveResultSets=true";
        public const string SQLite = @"Filename=./gra.db";
    }
}
