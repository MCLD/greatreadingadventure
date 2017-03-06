using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Linq;

namespace GRA.Domain.Repository.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> queryable, BaseFilter filter)
        {
            if (filter.Skip != null && filter.Take != null)
            {
                return queryable.Skip((int)filter.Skip).Take((int)filter.Take);
            }
            else
            {
                return queryable;
            }
        }
    }
}
