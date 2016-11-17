using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IProgramRepository : IRepository<Model.Program>
    {
        IQueryable<Model.Program> GetAll();
    }
}
