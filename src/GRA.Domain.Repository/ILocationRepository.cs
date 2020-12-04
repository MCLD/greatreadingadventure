﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<ICollection<Location>> GetAll(int siteId, bool requireGeolocation = false);
        Task<int> CountAsync(BaseFilter filter);
        Task<ICollection<Location>> PageAsync(BaseFilter filter);
        Task<bool> ValidateAsync(int locationId, int siteId);
    }
}
