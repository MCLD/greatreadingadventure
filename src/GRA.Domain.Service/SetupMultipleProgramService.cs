using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System;
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

        public SetupMultipleProgramService(ILogger<SetupMultipleProgramService> logger,
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
            };
            branch = await _branchRepository.AddSaveAsync(userId, branch);

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
                AgeMaximum = 4
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            var pointTranslation = new Model.PointTranslation
            {
                ActivityAmount = 1,
                ActivityDescription = "minute",
                ActivityDescriptionPlural = "minutes",
                IsSingleEvent = false,
                PointsEarned = 1,
                ProgramId = program.Id,
                TranslationName = "One minute, one point",
                TranslationDescriptionPastTense = "read {0}",
                TranslationDescriptionPresentTense = "reading {0}"
            };
            await _pointTranslationRepository.AddSaveAsync(userId, pointTranslation);

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
                AgeMinimum = 5
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            pointTranslation.ProgramId = program.Id;
            await _pointTranslationRepository.AddSaveAsync(userId, pointTranslation);

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
                AgeMinimum = 12
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            pointTranslation.ProgramId = program.Id;
            await _pointTranslationRepository.AddSaveAsync(userId, pointTranslation);

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
                AgeMinimum = 18
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            pointTranslation.ProgramId = program.Id;
            await _pointTranslationRepository.AddSaveAsync(userId, pointTranslation);

            // required for a user to be an administrator
            var adminRole = await _roleRepository.AddSaveAsync(userId, new Model.Role
            {
                Name = "System Administrator"
            });

            // add code to make first user system administrator
            await _authorizationCodeRepository.AddSaveAsync(userId, new Model.AuthorizationCode
            {
                Code = initialAuthorizationCode.Trim().ToLower(),
                Description = "Initial code to grant system administrator status.",
                IsSingleUse = true,
                RoleId = adminRole.Id,
                SiteId = siteId
            });

            // system permissions
            foreach (var value in Enum.GetValues(typeof(Model.Permission)))
            {
                await _roleRepository.AddPermissionAsync(userId, value.ToString());
            }
            await _roleRepository.SaveAsync();

            // add permissions to the admin role
            foreach (var value in Enum.GetValues(typeof(Model.Permission)))
            {
                await _roleRepository.AddPermissionToRoleAsync(userId,
                    adminRole.Id,
                    value.ToString());
            }
            await _roleRepository.SaveAsync();

            foreach (var value in Enum.GetValues(typeof(Model.ChallengeTaskType)))
            {
                await _challengeTaskRepository.AddChallengeTaskTypeAsync(userId,
                    value.ToString());
            }
            await _challengeTaskRepository.SaveAsync();
        }
    }
}