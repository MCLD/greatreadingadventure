using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ISocialHeaderRepository : IRepository<Model.SocialHeader>
    {
        public Task<int> CountAsync(BaseFilter filter);

        public Task<Model.SocialHeader> GetByDateAsync(DateTime asOf);

        public Task<ICollection<Model.SocialHeader>> PageAsync(BaseFilter filter);
    }
}