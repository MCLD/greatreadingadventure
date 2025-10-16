using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IReportRequestRepository : IRepository<ReportRequest>
    {
        Task<int> CountAsync(ReportRequestFilter filter);
        Task<ICollection<ReportRequestSummary>> PageAsync(ReportRequestFilter filter);
    }
}
