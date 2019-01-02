using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IPsBlackoutDateRepository : IRepository<PsBlackoutDate>
    {
        Task<PsBlackoutDate> GetByDateAsync(DateTime date);
        Task<ICollection<PsBlackoutDate>> GetAllAsync();
        Task<DataWithCount<ICollection<PsBlackoutDate>>> PageAsync(BaseFilter filter);
    }
}
