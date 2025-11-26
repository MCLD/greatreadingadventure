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
        private const string DefaultBannerFilename = "gra-forest.jpg";
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

            var systemUserId = await _userRepository.GetSystemUserId();

            if (exitLandingDetails == null)
            {
                // not fetched from cache, fetch from database
                try
                {
                    exitLandingDetails = await _exitLandingRepository.GetByIdAsync((int)siteStage);

                    if (exitLandingDetails != null)
                    {
                        // found in database

                        // check for banner alts
                        if (exitLandingDetails.BannerAlt == default)
                        {
                            exitLandingDetails.BannerAlt
                                = await InsertBannerAsync(nameof(ExitLandingDetails.BannerAlt),
                                    systemUserId,
                                    siteStage,
                                    languageId);
                            _logger.LogInformation("Inserted {Item} for {SiteStage} as segment {SegmentId}",
                                nameof(ExitLandingDetails.BannerAlt),
                                siteStage,
                                exitLandingDetails.BannerAlt);
                        }

                        // check for banner files
                        if (exitLandingDetails.BannerFile == default)
                        {
                            exitLandingDetails.BannerFile
                                = await InsertBannerAsync(nameof(ExitLandingDetails.BannerFile),
                                    systemUserId,
                                    siteStage,
                                    languageId);
                            _logger.LogInformation("Inserted {Item} for {SiteStage} as segment {SegmentId}",
                                nameof(ExitLandingDetails.BannerFile),
                                siteStage,
                                exitLandingDetails.BannerFile);
                        }

                        await _cache.SaveToCacheAsync(cacheKey,
                            exitLandingDetails,
                            CacheExitLandingHours);
                    }
                }
                catch (GraException) { }
            }

            // if it was not found in the database, insert the defaults
            exitLandingDetails ??= await InsertDefaultsAsync(systemUserId, siteStage);

            if (exitLandingDetails == null)
            {
                throw new GraException($"Unable to find exit/landing details for site stage {siteStage}");
            }

            return new ExitLandingDetails
            {
                BannerAlt = await _segmentService
                    .GetTextAsync(exitLandingDetails.BannerAlt, languageId),
                BannerFile = await _segmentService
                    .GetTextAsync(exitLandingDetails.BannerFile, languageId),
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
                    Message = _sharedLocalizer.GetString(language.Name,
                        Annotations.Home.BannerAltTag),
                    MessageName = nameof(ExitLandingMessageSet.BannerAlt)
                });

                result.Add(new DefaultMessage
                {
                    LanguageId = language.Id,
                    Message = DefaultBannerFilename,
                    MessageName = nameof(ExitLandingMessageSet.BannerFile)
                });

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

        private async Task<int> InsertBannerAsync(string whichItem,
            int systemUserId,
            SiteStage? currentStage,
            int? currentLanguageId)
        {
            var isAlt = whichItem.Equals(nameof(ExitLandingDetails.BannerAlt),
                StringComparison.OrdinalIgnoreCase);
            int returnValue = 0;

            foreach (var siteStage in Enum.GetValues<SiteStage>())
            {
                if (siteStage == SiteStage.Unknown)
                {
                    continue;
                }
                int? segmentId = null;

                string segmentName = siteStage.ToString().AddTitleCaseSpaces()
                    + " stage "
                    + (isAlt
                        ? nameof(ExitLandingDetails.BannerAlt)
                        : nameof(ExitLandingDetails.BannerFile));

                foreach (var language in await _languageService.GetActiveAsync())
                {
                    var text = isAlt
                        ? _sharedLocalizer.GetString(language.Name, Annotations.Home.BannerAltTag)
                        : DefaultBannerFilename;

                    if (segmentId.HasValue)
                    {
                        await _segmentService.UpdateTextAsync(segmentId.Value,
                            language.Id,
                            text);
                    }
                    else
                    {
                        var segment = await _segmentService.AddTextAsync(systemUserId,
                            language.Id,
                            SegmentType.LandingPage,
                            text,
                            segmentName);

                        segmentId = segment.SegmentId;

                        var exitLanding = await _exitLandingRepository.GetByIdAsync((int)siteStage);

                        if (isAlt)
                        {
                            exitLanding.BannerAlt = segmentId.Value;
                        }
                        else
                        {
                            exitLanding.BannerFile = segmentId.Value;
                        }

                        await _exitLandingRepository.UpdateSaveAsync(systemUserId, exitLanding);
                    }

                    if (siteStage == currentStage && language.Id == currentLanguageId)
                    {
                        returnValue = segmentId.Value;
                    }
                }
            }

            return returnValue;
        }

        private async Task<ExitLandingMessageSet> InsertDefaultsAsync(int systemUserId,
            SiteStage siteStage)
        {
            ExitLandingMessageSet value = null;
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
                            SegmentType.LandingPage,
                            message.Message,
                            segmentName);

                        switch (message.MessageName)
                        {
                            case nameof(ExitLandingMessageSet.BannerAlt):
                                exitLandingDetail.BannerAlt = segment.SegmentId;
                                break;

                            case nameof(ExitLandingMessageSet.BannerFile):
                                exitLandingDetail.BannerFile = segment.SegmentId;
                                break;

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
