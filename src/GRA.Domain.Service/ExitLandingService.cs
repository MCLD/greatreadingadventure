using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class ExitLandingService : BaseUserService<ExitLandingService>
    {
        private readonly IGraCache _cache;
        private readonly IExitLandingMessagesRepository _exitLandingRepository;
        private readonly LanguageService _languageService;
        private readonly SegmentService _segmentService;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        private readonly IUserRepository _userRepository;
        private readonly int CacheExitLandingHours;

        public ExitLandingService(ILogger<ExitLandingService> logger,
            IDateTimeProvider dateTimeProvider,
            IExitLandingMessagesRepository exitLandingRepository,
            IGraCache cache,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            LanguageService languageService,
            SegmentService segmentService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            CacheExitLandingHours = 8;

            ArgumentNullException.ThrowIfNull(cache);
            ArgumentNullException.ThrowIfNull(exitLandingRepository);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(segmentService);
            ArgumentNullException.ThrowIfNull(sharedLocalizer);
            ArgumentNullException.ThrowIfNull(userRepository);

            _cache = cache;
            _exitLandingRepository = exitLandingRepository;
            _languageService = languageService;
            _segmentService = segmentService;
            _sharedLocalizer = sharedLocalizer;
            _userRepository = userRepository;
        }

        public async Task<ExitLandingDetails> GetExitLandingDetailsAsync(SiteStage siteStage,
            int languageId)
        {
            var cacheKey = string.Format(CultureInfo.InvariantCulture,
                CacheKey.ExitLandingMessageSet,
                siteStage);

            // attempt to pull from cache
            var exitLandingDetails = await _cache
                .GetObjectFromCacheAsync<ExitLandingMessageSet>(cacheKey);

            if (exitLandingDetails == null)
            {
                try
                {
                    exitLandingDetails = await _exitLandingRepository.GetByIdAsync((int)siteStage);
                    if (exitLandingDetails != null)
                    {
                        await _cache.SaveToCacheAsync(cacheKey,
                            exitLandingDetails,
                            CacheExitLandingHours);
                    }
                }
                catch (GraException) { }
            }

            exitLandingDetails ??= await InsertDefaultsAsync(siteStage);

            if (exitLandingDetails == null)
            {
                throw new GraException($"Unable to find exit/landing details for site stage {siteStage}");
            }

            return new ExitLandingDetails
            {
                ExitLeftMessage = await _segmentService
                    .GetTextAsync(exitLandingDetails.ExitLeftMessage, languageId),
                LandingCenterMessage = await _segmentService
                    .GetTextAsync(exitLandingDetails.LandingCenterMessage, languageId),
                LandingLeftMessage = await _segmentService
                    .GetTextAsync(exitLandingDetails.LandingLeftMessage, languageId),
                LandingRightMessage = await _segmentService
                    .GetTextAsync(exitLandingDetails.LandingRightMessage, languageId)
            };
        }

        private async Task<IList<DefaultMessage>> GetDefaultsAsync()
        {
            var result = new List<DefaultMessage>();
            foreach (var language in await _languageService.GetActiveAsync())
            {
                result.Add(new DefaultMessage
                {
                    LanguageId = language.Id,
                    Message = "## "
                        + _sharedLocalizer.GetString(language.Name,
                            Annotations.Home.AdventureNotOver)
                        + Environment.NewLine
                        + Environment.NewLine
                        + _sharedLocalizer.GetString(language.Name,
                            Annotations.Home.ContinueYourAdventure),
                    MessageName = nameof(ExitLandingMessageSet.ExitLeftMessage)
                });

                foreach (var siteStage in Enum.GetValues<SiteStage>())
                {
                    switch (siteStage)
                    {
                        case SiteStage.BeforeRegistration:
                            result.Add(new DefaultMessage
                            {
                                LanguageId = language.Id,
                                Message = _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.WelcomeTo, Tags.ExitLandingTags.Sitename)
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + _sharedLocalizer.GetString(language.Name,
                                        Annotations.Title.RegistrationOpens,
                                            Tags.ExitLandingTags.Sitename,
                                            Tags.ExitLandingTags.RegistrationOpens),
                                MessageName = nameof(ExitLandingMessageSet.LandingCenterMessage),
                                SiteStage = siteStage
                            });
                            break;

                        case SiteStage.RegistrationOpen:
                            result.Add(new DefaultMessage
                            {
                                LanguageId = language.Id,
                                Message = _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.WelcomeTo, Tags.ExitLandingTags.Sitename)
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.StartAdventure),
                                MessageName = nameof(ExitLandingMessageSet.LandingCenterMessage),
                                SiteStage = siteStage
                            });
                            break;

                        case SiteStage.ProgramEnded:
                            result.Add(new DefaultMessage
                            {
                                LanguageId = language.Id,
                                Message = _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.Thanks, Tags.ExitLandingTags.Sitename)
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.StillSignIn),
                                MessageName = nameof(ExitLandingMessageSet.LandingCenterMessage),
                                SiteStage = siteStage
                            });
                            break;

                        case SiteStage.AccessClosed:
                            result.Add(new DefaultMessage
                            {
                                LanguageId = language.Id,
                                Message = _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.Thanks, Tags.ExitLandingTags.Sitename),
                                MessageName = nameof(ExitLandingMessageSet.LandingCenterMessage),
                                SiteStage = siteStage
                            });
                            break;

                        default:
                            result.Add(new DefaultMessage
                            {
                                LanguageId = language.Id,
                                Message = _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.WelcomeTo, Tags.ExitLandingTags.Sitename)
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + _sharedLocalizer.GetString(language.Name,
                                        Annotations.Home.StartAdventure),
                                MessageName = nameof(ExitLandingMessageSet.LandingCenterMessage),
                                SiteStage = siteStage
                            });
                            break;
                    }
                }

                result.Add(new DefaultMessage
                {
                    LanguageId = language.Id,
                    Message = _sharedLocalizer.GetString(language.Name,
                            Annotations.Home.ForMoreInformation)
                        + Environment.NewLine
                        + Environment.NewLine
                        + _sharedLocalizer.GetString(language.Name,
                            Annotations.Home.GoOnAJourney),
                    MessageName = nameof(ExitLandingMessageSet.LandingLeftMessage)
                });

                result.Add(new DefaultMessage
                {
                    LanguageId = language.Id,
                    Message = _sharedLocalizer.GetString(language.Name,
                            Annotations.Home.Read20)
                        + Environment.NewLine
                        + Environment.NewLine
                        + _sharedLocalizer.GetString(language.Name,
                            Annotations.Home.ReadingIsFundamental),
                    MessageName = nameof(ExitLandingMessageSet.LandingRightMessage)
                });
            }
            return result;
        }

        private async Task<ExitLandingMessageSet> InsertDefaultsAsync(SiteStage siteStage)
        {
            ExitLandingMessageSet value = null;
            var systemUserId = await _userRepository.GetSystemUserId();
            var defaultLanguageId = await _languageService.GetDefaultLanguageIdAsync();
            var defaultMessages = await GetDefaultsAsync();

            foreach (var loopSiteStage in Enum.GetValues<SiteStage>())
            {
                ExitLandingMessageSet exitLandingDetail = null;
                if (loopSiteStage != siteStage)
                {
                    // check if it's already in the database
                    try
                    {
                        exitLandingDetail = await _exitLandingRepository
                            .GetByIdAsync((int)loopSiteStage);
                    }
                    catch (GraException) { }
                }

                if (exitLandingDetail == null)
                {
                    // if not, insert it and related segments
                    exitLandingDetail = new ExitLandingMessageSet();

                    var thisStageMessages = defaultMessages.Where(_ => _.SiteStage == loopSiteStage
                        || !_.SiteStage.HasValue);

                    foreach (var message in thisStageMessages
                        .Where(_ => _.LanguageId == defaultLanguageId))
                    {
                        string segmentName = loopSiteStage.ToString().AddTitleCaseSpaces()
                            + " stage "
                            + message.MessageName.AddTitleCaseSpaces();

                        var segment = await _segmentService.AddTextAsync(systemUserId,
                            message.LanguageId,
                            message.Message,
                            segmentName);

                        switch (message.MessageName)
                        {
                            case nameof(ExitLandingMessageSet.ExitLeftMessage):
                                exitLandingDetail.ExitLeftMessage = segment.SegmentId;
                                break;

                            case nameof(ExitLandingMessageSet.LandingCenterMessage):
                                exitLandingDetail.LandingCenterMessage = segment.SegmentId;
                                break;

                            case nameof(ExitLandingMessageSet.LandingLeftMessage):
                                exitLandingDetail.LandingLeftMessage = segment.SegmentId;
                                break;

                            case nameof(ExitLandingMessageSet.LandingRightMessage):
                                exitLandingDetail.LandingRightMessage = segment.SegmentId;
                                break;

                            default:
                                throw new GraException($"Invalid message name: {message.MessageName}");
                        }

                        foreach (var otherLanguage in thisStageMessages
                            .Where(_ => _.LanguageId != defaultLanguageId
                                && _.MessageName == message.MessageName))
                        {
                            await _segmentService.UpdateTextAsync(segment.SegmentId,
                                otherLanguage.LanguageId,
                                otherLanguage.Message);
                        }
                    }

                    exitLandingDetail.CreatedBy = systemUserId;
                    exitLandingDetail.CreatedAt = _dateTimeProvider.Now;

                    _logger.LogInformation("Adding exit/landing details for: {SiteStage}",
                        loopSiteStage);
                    exitLandingDetail = await _exitLandingRepository
                        .AddSaveAsync(systemUserId, exitLandingDetail);
                }

                if (loopSiteStage == siteStage)
                {
                    value = exitLandingDetail;
                }
            }
            return value;
        }

        private class DefaultMessage
        {
            public int LanguageId { get; set; }
            public string Message { get; set; }
            public string MessageName { get; set; }
            public SiteStage? SiteStage { get; set; }
        }
    }
}
