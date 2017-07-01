using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IDrawingRepository : IRepository<Drawing>
    {
        Task<IEnumerable<Drawing>> PageAllAsync(DrawingFilter filter);
        Task<int> GetCountAsync(DrawingFilter filter);
        Task<Drawing> GetByIdAsync(int id, int skip, int take);
        Task<int> GetWinnerCountAsync(int id);
        Task SetArchivedAsync(int userId, int drawingId, bool archive);
    }
}
