using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IEmailTemplateRepository : IRepository<Model.EmailTemplate>
    {
        public Task<DataWithCount<ICollection<EmailTemplate>>> PageAsync(BaseFilter filter);
    }
}
