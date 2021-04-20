using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class SocialManagementService : BaseUserService<SocialManagementService>
    {
        // if this is updated here, SocialService must be updated as well
        private const int CacheInHours = 4;

        private const string SocialPath = "social";

        private readonly IDistributedCache _cache;
        private readonly LanguageService _languageService;
        private readonly IPathResolver _pathResolver;
        private readonly ISocialHeaderRepository _socialHeaderRepository;
        private readonly ISocialRepository _socialRepository;

        public SocialManagementService(ILogger<SocialManagementService> logger,
           IDateTimeProvider dateTimeProvider,
           IDistributedCache cache,
           IPathResolver pathResolver,
           IUserContextProvider userContextProvider,
           ISocialHeaderRepository socialHeaderRepository,
           ISocialRepository socialRepository,
           LanguageService languageService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            SetManagementPermission(Permission.ManageSocial);

            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
            _pathResolver = pathResolver
                ?? throw new ArgumentNullException(nameof(pathResolver));
            _socialHeaderRepository = socialHeaderRepository
                ?? throw new ArgumentNullException(nameof(socialHeaderRepository));
            _socialRepository = socialRepository
                ?? throw new ArgumentNullException(nameof(socialRepository));
        }

        public async Task<int> AddHeaderAndSocialAsync(SocialHeader header,
            string filename,
            byte[] imageBytes)
        {
            VerifyManagementPermission();

            var insertedHeader
                = await _socialHeaderRepository.AddSaveAsync(GetActiveUserId(), header);

            var social = header?.Socials.FirstOrDefault();
            if (social != null)
            {
                social.SocialHeaderId = insertedHeader.Id;
                await AddSocialAsync(social, filename, imageBytes);

                await ClearSocialCache(insertedHeader.Id, social.LanguageId);
            }

            return insertedHeader.Id;
        }

        public async Task AddSocialAsync(Social social, string filename, byte[] imageBytes)
        {
            VerifyManagementPermission();

            if (social == null)
            {
                return;
            }

            (social.ImageLink, social.ImageWidth, social.ImageHeight)
                = HandleSocialImage(filename, imageBytes);

            await _socialRepository.AddSaveAsync(social);

            var socialHeader = await _socialHeaderRepository.GetByIdAsync(social.SocialHeaderId);

            await ClearSocialCache(socialHeader.Id, social.LanguageId);
        }

        public async Task<int?> DeleteSocial(int socialHeaderId, int languageId)
        {
            VerifyManagementPermission();

            var socials = await _socialRepository.GetByHeaderIdsAsync(new[] { socialHeaderId });

            var toDelete = socials
                .Single(_ => _.SocialHeaderId == socialHeaderId && _.LanguageId == languageId);

            try
            {
                var filename = toDelete.ImageLink[toDelete.ImageLink.LastIndexOf('/')..];
                File.Delete(GetFilePath(filename));
            }
            catch (IOException ioex)
            {
                _logger.LogError("Problem deleting social file: {ErrorMessage}",
                    ioex.Message);
            }

            await _socialRepository.RemoveSaveAsync(socialHeaderId, languageId);

            int? returnValue = null;

            if (!socials.Except(new[] { toDelete }).Any())
            {
                await _socialHeaderRepository.RemoveSaveAsync(GetActiveUserId(), socialHeaderId);
            }
            else
            {
                returnValue = socialHeaderId;
            }

            await ClearSocialCache(socialHeaderId, languageId);

            return returnValue;
        }

        public async Task<SocialHeader> GetHeaderAndSocialAsync(int headerId, int languageId)
        {
            VerifyManagementPermission();

            var header = await _socialHeaderRepository.GetByIdAsync(headerId);

            header.Socials = new[] {
               await _socialRepository.GetByHeaderLanguageAsync(headerId, languageId)
            };

            return header;
        }

        public async Task<DataWithCount<ICollection<SocialHeader>>>
                    GetPaginatedListAsync(BaseFilter filter)
        {
            VerifyManagementPermission();

            if (filter == null)
            {
                filter = new BaseFilter();
            }

            filter.SiteId = GetCurrentSiteId();

            var languages = await _languageService.GetActiveAsync();

            var headers = await _socialHeaderRepository.PageAsync(filter);

            var socials = await _socialRepository.GetByHeaderIdsAsync(headers.Select(_ => _.Id));

            foreach (var header in headers)
            {
                header.Socials = socials.Where(_ => _.SocialHeaderId == header.Id).ToList();

                foreach (var social in header.Socials)
                {
                    social.Language = languages.SingleOrDefault(_ => _.Id == social.LanguageId);
                }
            }

            return new DataWithCount<ICollection<SocialHeader>>
            {
                Data = headers,
                Count = await _socialHeaderRepository.CountAsync(filter)
            };
        }

        public async Task ReplaceImageAsync(int socialHeaderId,
            int languageId,
            string filename,
            byte[] imageBytes)
        {
            VerifyManagementPermission();

            var social = await _socialRepository
                .GetByHeaderLanguageAsync(socialHeaderId, languageId);

            if (social == null)
            {
                return;
            }

            (social.ImageLink, social.ImageWidth, social.ImageHeight)
                = HandleSocialImage(filename, imageBytes);

            await _socialRepository.UpdateSaveAsync(social);

            await ClearSocialCache(socialHeaderId, languageId);
        }

        public async Task UpdateHeaderAsync(int headerId, string name, DateTime startDate)
        {
            VerifyManagementPermission();

            var header = await _socialHeaderRepository.GetByIdAsync(headerId);
            header.Name = name;
            header.StartDate = startDate;
            await _socialHeaderRepository.UpdateSaveAsync(GetActiveUserId(), header);

            var socials = await _socialRepository.GetByHeaderIdsAsync(new[] { headerId });
            foreach (var social in socials)
            {
                await ClearSocialCache(headerId, social.LanguageId);
            }
        }

        public async Task UpdateSocialAsync(Social social)
        {
            VerifyManagementPermission();

            if (social == null)
            {
                return;
            }

            var dbSocial = await _socialRepository.GetByHeaderLanguageAsync(social.SocialHeaderId,
                social.LanguageId);

            dbSocial.Description = social.Description;
            dbSocial.ImageAlt = social.ImageAlt;
            dbSocial.Title = social.Title;
            dbSocial.TwitterUsername = social.TwitterUsername;

            await _socialRepository.UpdateSaveAsync(dbSocial);

            await ClearSocialCache(social.SocialHeaderId, social.LanguageId);
        }

        private async Task ClearSocialCache(int headerId, int languageId)
        {
            await _cache.RemoveAsync(CacheKey.SocialHeader);
            await _cache.RemoveAsync(GetCacheKey(CacheKey.Social, headerId, languageId));
        }

        private string GetFilePath(string filename)
        {
            string contentDir = _pathResolver
                .ResolveContentFilePath(Path.Combine($"site{GetCurrentSiteId()}", SocialPath));
            Directory.CreateDirectory(contentDir);
            return Path.Combine(contentDir, filename);
        }

        private string GetLinkPath(string filename)
        {
            return _pathResolver
                .ResolveContentPath($"site{GetCurrentSiteId()}/{SocialPath}/{filename}");
        }

        private (string ImageLink, int ImageWidth, int ImageHeight) HandleSocialImage(
            string filename,
            byte[] imageBytes)
        {
            var fullPath = GetFilePath(filename);
            int dupeCheck = 1;
            while (File.Exists(fullPath))
            {
                filename = Path.GetFileNameWithoutExtension(filename)
                        + $"-{dupeCheck++}"
                        + Path.GetExtension(filename);
                fullPath = GetFilePath(filename);
            }

            File.WriteAllBytes(fullPath, imageBytes);

            using var image = SixLabors.ImageSharp.Image.Load(imageBytes);

            return (GetLinkPath(filename), image.Width, image.Height);
        }
    }
}