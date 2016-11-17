using System.Linq;

namespace GRA.Domain.Repository
{
    public interface ISiteRepository : IRepository<Model.Site>
    {
        IQueryable<Model.Site> GetAll();
    }
}
