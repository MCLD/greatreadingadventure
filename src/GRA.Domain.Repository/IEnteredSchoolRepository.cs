using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IEnteredSchoolRepository : IRepository<Model.EnteredSchool>
    {
        Task ConvertSchoolAsync(int userId, int enteredSchoolId, int schoolId);
    }
}
