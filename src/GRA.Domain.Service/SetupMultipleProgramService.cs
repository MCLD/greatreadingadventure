﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

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
        private readonly IUserRepository _userRepository;

        public SetupMultipleProgramService(ILogger<SetupMultipleProgramService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBranchRepository branchRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IProgramRepository programRepository,
            IRoleRepository roleRepository,
            ISystemRepository systemRepository,
            IPointTranslationRepository pointTranslationRepository,
            IUserRepository userRepository) : base(logger, dateTimeProvider)
        {
            _authorizationCodeRepository = authorizationCodeRepository 
                ?? throw new ArgumentNullException(nameof(authorizationCodeRepository));
            _branchRepository = branchRepository 
                ?? throw new ArgumentNullException(nameof(branchRepository));
            _challengeTaskRepository = challengeTaskRepository 
                ?? throw new ArgumentNullException(nameof(challengeTaskRepository));
            _programRepository = programRepository 
                ?? throw new ArgumentNullException(nameof(programRepository));
            _roleRepository = roleRepository 
                ?? throw new ArgumentNullException(nameof(roleRepository));
            _systemRepository = systemRepository 
                ?? throw new ArgumentNullException(nameof(systemRepository));
            _pointTranslationRepository = pointTranslationRepository 
                ?? throw new ArgumentNullException(nameof(pointTranslationRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task InsertAsync(int siteId, string initialAuthorizationCode)
        {
            int userId = Defaults.InitialInsertUserId;

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
                Address = "The Geographic Center, Lebanon, KS 66952"
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

            var programIds = new List<int>();
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
            programIds.Add(program.Id);

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
            programIds.Add(program.Id);

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
            programIds.Add(program.Id);

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
            programIds.Add(program.Id);

            // insert system user
            userId = await _userRepository.GetSystemUserId();

            await _systemRepository.UpdateCreatedByAsync(userId, system.Id);
            await _branchRepository.UpdateCreatedByAsync(userId, branch.Id);
            await _pointTranslationRepository.UpdateCreatedByAsync(userId, pointTranslation.Id);
            await _programRepository.UpdateCreatedByAsync(userId, programIds.ToArray());

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
            await _roleRepository.AddPermissionListAsync(userId, permissionList);
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