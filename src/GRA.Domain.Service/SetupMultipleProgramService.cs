using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class SetupMultipleProgramService
        : BaseService<SetupMultipleProgramService>, IInitialSetupService
    {
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;

        // Temp
        private readonly IPsAgeGroupRepository _psAgeGroupRepository;
        private readonly IPsBlackoutDateRepository _psBlackoutDateRepository;
        private readonly IPsSettingsRepository _psSettingsRepository;

        public SetupMultipleProgramService(ILogger<SetupMultipleProgramService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBranchRepository branchRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IProgramRepository programRepository,
            IRoleRepository roleRepository,
            ISystemRepository systemRepository,
            IPointTranslationRepository pointTranslationRepository,
            // Temp
            IPsAgeGroupRepository psAgeGroupRepository,
            IPsBlackoutDateRepository psBlackoutDateRepository,
            IPsSettingsRepository psSettingsRepository) : base(logger, dateTimeProvider)
        {
            _authorizationCodeRepository = Require.IsNotNull(authorizationCodeRepository,
                nameof(authorizationCodeRepository));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
            _programRepository = Require.IsNotNull(programRepository,
                nameof(programRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
            _pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));

            // Temp
            _psAgeGroupRepository = psAgeGroupRepository
                ?? throw new ArgumentNullException(nameof(psAgeGroupRepository));
            _psBlackoutDateRepository = psBlackoutDateRepository
                ?? throw new ArgumentNullException(nameof(psBlackoutDateRepository));
            _psSettingsRepository = psSettingsRepository 
                ?? throw new ArgumentNullException(nameof(psSettingsRepository));
        }

        public async Task InsertAsync(int siteId, string initialAuthorizationCode, int userId = -1)
        {
            //_config[ConfigurationKey.InitialAuthorizationCode]
            // this is the data required for a user to register
            var system = new Model.System
            {
                SiteId = siteId,
                Name = "Library District"
            };
            system = await _systemRepository.AddSaveAsync(userId, system);

            var branch = new Model.Branch
            {
                SystemId = system.Id,
                Name = "Main Library",
            };
            branch = await _branchRepository.AddSaveAsync(userId, branch);

            var pointTranslation = new Model.PointTranslation
            {
                ActivityAmount = 1,
                ActivityDescription = "minute",
                ActivityDescriptionPlural = "minutes",
                IsSingleEvent = false,
                PointsEarned = 1,
                SiteId = siteId,
                TranslationName = "One minute, one point",
                TranslationDescriptionPastTense = "read {0}",
                TranslationDescriptionPresentTense = "reading {0}"
            };
            pointTranslation = await _pointTranslationRepository.AddSaveAsync(userId,
                pointTranslation);

            int programCount = 0;
            var program = new Model.Program
            {
                SiteId = siteId,
                AchieverPointAmount = 1000,
                Name = "Prereaders (ages 4 and below)",
                Position = programCount++,
                AgeRequired = true,
                AskAge = true,
                SchoolRequired = false,
                AskSchool = false,
                AgeMaximum = 4,
                PointTranslationId = pointTranslation.Id
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            program = new Model.Program
            {
                SiteId = siteId,
                AchieverPointAmount = 1000,
                Name = "Kids (ages 5 to 11)",
                Position = programCount++,
                AgeRequired = true,
                AskAge = true,
                SchoolRequired = true,
                AskSchool = true,
                AgeMaximum = 11,
                AgeMinimum = 5,
                PointTranslationId = pointTranslation.Id
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            program = new Model.Program
            {
                SiteId = siteId,
                AchieverPointAmount = 1000,
                Name = "Teens (ages 12 to 17)",
                Position = programCount++,
                AgeRequired = true,
                AskAge = true,
                SchoolRequired = false,
                AskSchool = true,
                AgeMaximum = 17,
                AgeMinimum = 12,
                PointTranslationId = pointTranslation.Id
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            program = new Model.Program
            {
                SiteId = siteId,
                AchieverPointAmount = 1000,
                Name = "Adults (ages 18 and up)",
                Position = programCount,
                AgeRequired = false,
                AskAge = false,
                SchoolRequired = false,
                AskSchool = false,
                AgeMinimum = 18,
                PointTranslationId = pointTranslation.Id
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            // required for a user to be an administrator
            var adminRole = await _roleRepository.AddSaveAsync(userId, new Model.Role
            {
                Name = "System Administrator",
                IsAdmin = true
            });

            // add code to make first user system administrator
            await _authorizationCodeRepository.AddSaveAsync(userId, new Model.AuthorizationCode
            {
                Code = initialAuthorizationCode.Trim().ToLower(),
                Description = "Initial code to grant system administrator status.",
                IsSingleUse = false,
                RoleId = adminRole.Id,
                SiteId = siteId
            });

            // set up system permissions and add to the admin role
            var permissionList = Enum.GetValues(typeof(Model.Permission))
                    .Cast<Model.Permission>()
                    .Select(_ => _.ToString());
            await _roleRepository.AddPermissionListAsync(permissionList);
            await _roleRepository.SaveAsync();

            foreach (var value in Enum.GetValues(typeof(Model.ChallengeTaskType)))
            {
                await _challengeTaskRepository.AddChallengeTaskTypeAsync(userId,
                    value.ToString());
            }
            await _challengeTaskRepository.SaveAsync();


            // Temp for performer registration

            var psSettings = new Model.PsSettings
            {
                SiteId = siteId,
                ContactEmail = "DanielWilcox@mcldaz.org",
                SelectionsPerBranch = 3,
                RegistrationOpen = DateTime.Parse("2018-10-20"),
                RegistrationClosed = DateTime.Parse("2018-10-30"),
                SchedulingPreview = DateTime.Parse("2018-11-07"),
                SchedulingOpen = DateTime.Parse("2018-11-10"),
                SchedulingClosed = DateTime.Parse("2018-11-20"),
                SchedulePosted = DateTime.Parse("2018-11-30"),
                ScheduleStartDate = DateTime.Parse("2019-01-01"),
                ScheduleEndDate = DateTime.Parse("2019-03-01")
            };
            await _psSettingsRepository.AddSaveAsync(userId, psSettings);

            var psBlackoutDate = new Model.PsBlackoutDate
            {
                Date = DateTime.Parse("2019-01-15"),
                Reason = "test"
            };
            await _psBlackoutDateRepository.AddSaveAsync(userId, psBlackoutDate);

            var psAgeGroup = new Model.PsAgeGroup
            {
                Name = "Adult"
            };
            await _psAgeGroupRepository.AddSaveAsync(userId, psAgeGroup);
        }
    }
}