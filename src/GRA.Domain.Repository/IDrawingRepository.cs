using GRA.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IDrawingRepository : IRepository<Drawing>
    {
        Task AddWinnerAsync(DrawingWinner winners);
        Task<IEnumerable<Drawing>> PageAllAsync(int siteId, int skip, int take);
        Task<int> GetCountAsync(int siteId);
        Task<Drawing> GetByIdAsync(int id, int skip, int take);
        Task<int> GetWinnerCountAsync(int id);
        Task RemoveWinnerAsync(int drawingId, int userId);
        Task RedeemWinnerAsync(int drawingId, int userId);
        Task UndoRedemptionAsync(int drawingId, int userid);
        Task<IEnumerable<DrawingWinner>> PageUserAsync(int userId, int skip, int take);
        Task<int> GetUserWinCountAsync(int userId);
    }
}
