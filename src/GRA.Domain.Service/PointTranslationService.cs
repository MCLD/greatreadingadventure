using System;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class PointTranslationService : BaseUserService<PointTranslationService>
    {
        private readonly IPointTranslationRepository _pointTranslationRepository;
        public PointTranslationService(ILogger<PointTranslationService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IPointTranslationRepository pointTranslationRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _pointTranslationRepository = pointTranslationRepository 
                ?? throw new ArgumentNullException(nameof(pointTranslationRepository));
        }

        public async Task<PointTranslation> GetByProgramIdAsync(int id)
        {
            return await _pointTranslationRepository.GetByProgramIdAsync(id);
        }
    }
}
