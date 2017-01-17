using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service.Abstract
{
    public interface IInitialSetupService
    {
        Task InsertAsync(int siteId, string initialAuthorizationCode, int userId = -1);
    }
}
