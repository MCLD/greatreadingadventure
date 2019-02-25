using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class SetupSingleProgramService
        : BaseService<SetupSingleProgramService>, IInitialSetupService
    {
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;

        public SetupSingleProgramService(ILogger<SetupSingleProgramService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBranchRepository branchRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IProgramRepository programRepository,
            IRoleRepository roleRepository,
            ISystemRepository systemRepository,
            IPointTranslationRepository pointTranslationRepository) : base(logger, dateTimeProvider)
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
                Address = "123 Test Street"
            };
            branch = await _branchRepository.AddSaveAsync(userId, branch);

            var pointTranslation = new Model.PointTranslation
            {
                ActivityAmount = 1,
                ActivityDescription = "book",
                ActivityDescriptionPlural = "books",
                IsSingleEvent = true,
                PointsEarned = 10,
                SiteId = siteId,
                TranslationName = "One book, ten points",
                TranslationDescriptionPastTense = "read {0}",
                TranslationDescriptionPresentTense = "reading {0}"
            };
            pointTranslation = await _pointTranslationRepository.AddSaveAsync(userId,
                pointTranslation);

            var program = new Model.Program
            {
                SiteId = siteId,
                AchieverPointAmount = 100,
                Name = "Reading Program",
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
                if ((Model.ChallengeTaskType)value == Model.ChallengeTaskType.Book)
                {
                    await _challengeTaskRepository.AddChallengeTaskTypeAsync(userId,
                        value.ToString(),
                        1,
                        pointTranslation.Id);
                }
                else
                {
                    await _challengeTaskRepository.AddChallengeTaskTypeAsync(userId,
                        value.ToString());
                }
            }
            await _challengeTaskRepository.SaveAsync();
        }
    }
}