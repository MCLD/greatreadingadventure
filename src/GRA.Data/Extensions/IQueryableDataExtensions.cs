using System;
using System.Linq;
using GRA.Data.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Data.Extensions
{
    public static class IQueryableDataExtensions
    {
        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> queryable,
            ChallengeFilter filter)
            where T : Challenge
        {
            if (filter == null) { throw new ArgumentNullException(nameof(filter)); }
            return filter.Ordering switch
            {
                ChallengeFilter.OrderingOption.Name => queryable.OrderBy(_ => _.Name),
                ChallengeFilter.OrderingOption.Recent => queryable
                    .OrderByDescending(_ => _.CreatedAt)
                    .ThenBy(_ => _.Name),
                _ => queryable
                    .OrderByDescending(_ => _.EstimatedPopularity)
                    .ThenByDescending(_ => _.CreatedAt)
            };
        }
    }
}
