using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Repository;

namespace GRA.Data.Repository
{
    public class SystemInformationRepository : ISystemInformationRepository
    {
        private readonly Context _context;
        public SystemInformationRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<string> GetCurrentMigrationAsync()
        {
            return await _context.GetCurrentMigrationAsync();
        }

        public async Task<IEnumerable<string>> GetMigrationsListAsync()
        {
            return await _context.GetMigrationsListAsync();
        }
    }
}
