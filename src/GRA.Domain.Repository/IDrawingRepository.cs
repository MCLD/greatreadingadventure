using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IDrawingRepository : IRepository<Drawing>
    {
        Task<int> GetCountAsync(DrawingFilter filter);

        Task<Drawing> GetDetailsWinners(int id, int skip, int take);

        Task<Drawing> GetDetailsWinners(int id);

        Task<int> GetWinnerCountAsync(int id);

        Task<IEnumerable<Drawing>> PageAllAsync(DrawingFilter filter);

        Task SetArchivedAsync(int userId, int drawingId, bool archive);
    }
}
