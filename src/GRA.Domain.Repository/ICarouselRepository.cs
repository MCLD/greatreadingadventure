using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ICarouselRepository : IRepository<Model.Carousel>
    {
        Task<Model.Carousel> GetCurrentDashboardAsync(BaseFilter filter);
        Task<ICollection<Model.Carousel>> PageAsync(BaseFilter filter);
        Task<int> CountAsync(BaseFilter filter);
    }
}
