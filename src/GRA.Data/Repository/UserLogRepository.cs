using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;

namespace GRA.Data.Repository
{
    public class UserLogRepository
        : AuditingRepository<Model.UserLog, Domain.Model.UserLog>, IUserLogRepository
    {
        public UserLogRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserLogRepository> logger) : base(repositoryFacade, logger)
        {
        }
    }
}
