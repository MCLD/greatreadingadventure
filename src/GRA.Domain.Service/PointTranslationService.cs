using System;
using System.Linq;
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

        public async Task<PointTranslation> GetByProgramIdAsync(int id, bool titleCase = false)
        {
            var pointTranslation = await _pointTranslationRepository.GetByProgramIdAsync(id);

            if (titleCase)
            {
                if (!string.IsNullOrWhiteSpace(pointTranslation.ActivityDescription))
                {
                    pointTranslation.ActivityDescription = pointTranslation.ActivityDescription
                        .First()
                        .ToString()
                        .ToUpper() 
                        + pointTranslation.ActivityDescription.Substring(1);
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.ActivityDescriptionPlural))
                {
                    pointTranslation.ActivityDescriptionPlural = 
                        pointTranslation.ActivityDescriptionPlural
                        .First()
                        .ToString()
                        .ToUpper()
                        + pointTranslation.ActivityDescriptionPlural.Substring(1);
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.TranslationDescriptionPastTense))
                {
                    pointTranslation.TranslationDescriptionPastTense =
                        pointTranslation.TranslationDescriptionPastTense
                        .First()
                        .ToString()
                        .ToUpper()
                        + pointTranslation.TranslationDescriptionPastTense.Substring(1);
                }
                if (!string.IsNullOrWhiteSpace(pointTranslation.TranslationDescriptionPresentTense))
                {
                    pointTranslation.TranslationDescriptionPresentTense =
                        pointTranslation.TranslationDescriptionPresentTense
                        .First()
                        .ToString()
                        .ToUpper()
                        + pointTranslation.TranslationDescriptionPresentTense.Substring(1);
                }
            }
            return pointTranslation;
        }
    }
}
