using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ICarouselItemRepository : IRepository<Model.CarouselItem>
    {
        Task<IEnumerable<Model.CarouselItem>> GetByCarouselIdAsync(int carouselId);
        Task<IEnumerable<Model.CarouselItem>> GetByCarouselIdNoDescriptionAsync(int carouselId);
        Task<int> GetCountByCarouselIdAsync(int carouselId);
    }
}
