using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface ISystemRepository : IRepository<Model.System>
    {
        IQueryable<Model.System> GetAll();
    }
}
