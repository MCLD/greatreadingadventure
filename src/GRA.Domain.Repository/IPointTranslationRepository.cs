using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IPointTranslationRepository : IRepository<Model.PointTranslation>
    {
        Task<Model.PointTranslation> GetByProgramIdAsync(int programId);
    }
}