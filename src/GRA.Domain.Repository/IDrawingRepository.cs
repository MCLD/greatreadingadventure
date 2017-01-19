using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDrawingRepository : IRepository<Drawing>
    {
        Task AddWinnerAsync(DrawingWinner winners);
        Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take, bool archived);
        Task<int> GetCountAsync(int siteId, bool archived);
        Task<Drawing> GetByIdAsync(int id, int skip, int take);
        Task<int> GetWinnerCountAsync(int id);
        Task<DrawingWinner> GetDrawingWinnerById(int drawingId, int userId);
        Task RemoveWinnerAsync(int drawingId, int userId);
        Task RedeemWinnerAsync(int drawingId, int userId);
        Task UndoRedemptionAsync(int drawingId, int userid);
        Task<IEnumerable<DrawingWinner>> PageUserAsync(int userId, int skip, int take);
        Task<int> GetUserWinCountAsync(int userId);
        Task SetArchivedAsync(int userId, int drawingId, bool archive);
    }
}
