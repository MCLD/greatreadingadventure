using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDrawingRepository : IRepository<Drawing>
    {
        Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take, bool archived);
        Task<int> GetCountAsync(int siteId, bool archived);
        Task<Drawing> GetByIdAsync(int id, int skip, int take);
        Task<int> GetWinnerCountAsync(int id);
        Task SetArchivedAsync(int userId, int drawingId, bool archive);
    }
}
