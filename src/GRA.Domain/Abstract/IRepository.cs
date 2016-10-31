using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain
{
    public interface IRepository : IDisposable
    {
        IEnumerable<Model.Site> GetSites();
    }
}
