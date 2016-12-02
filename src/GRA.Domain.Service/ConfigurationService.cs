using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using System.Threading.Tasks;
using GRA.Domain.Model;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace GRA.Domain.Service
{
    public class ConfigurationService : Abstract.BaseService<ConfigurationService>
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _config;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISystemRepository _systemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPointTranslationRepository _pointTranslationRepository;
        private readonly ActivityService _activityService;
        public ConfigurationService(ILogger<ConfigurationService> logger,
            IConfigurationRoot config,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IBookRepository bookRepository,
            IBranchRepository branchRepository,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IMailRepository mailRepository,
            IProgramRepository programRepository,
            IRoleRepository roleRepository,
            ISiteRepository siteRepository,
            ISystemRepository systemRepository,
            IUserRepository userRepository,
            IPointTranslationRepository pointTranslationRepository,
            ActivityService activityService) : base(logger)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _config = Require.IsNotNull(config, nameof(config));
            _authorizationCodeRepository = Require.IsNotNull(authorizationCodeRepository, nameof(authorizationCodeRepository));
            _bookRepository = Require.IsNotNull(bookRepository, nameof(bookRepository));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
            _programRepository = Require.IsNotNull(programRepository,
                nameof(programRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _pointTranslationRepository = Require.IsNotNull(pointTranslationRepository,
                nameof(pointTranslationRepository));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
        }

        public async Task<bool> NeedsInitialSetupAsync()
        {
            var firstSite = await _siteRepository.GetAllAsync();
            return firstSite.Count() == 0 || firstSite.First().CreatedBy == -1;
        }

        public async Task InsertSetupData(Site site)
        {
            int userId = -1;

            // this is the data required for a user to register
            var system = new Model.System
            {
                SiteId = site.Id,
                Name = "Maricopa County Library District"
            };
            system = await _systemRepository.AddSaveAsync(userId, system);

            var branch = new Model.Branch
            {
                SiteId = site.Id,
                SystemId = system.Id,
                Name = "Admin",
                Address = "2700 N. Central Ave Ste 700, 85004",
                Telephone = "602-652-3064",
                Url = "http://mcldaz.org/"
            };
            branch = await _branchRepository.AddSaveAsync(userId, branch);

            var program = new Model.Program
            {
                SiteId = site.Id,
                AchieverPointAmount = 1000,
                Name = "Winter Reading Program",
            };
            program = await _programRepository.AddSaveAsync(userId, program);

            var pointTranslation = new Model.PointTranslation
            {
                ActivityAmount = 1,
                ActivityDescription = "book",
                IsSingleEvent = true,
                PointsEarned = 10,
                ProgramId = program.Id,
                TranslationName = "One book, ten points",
                TranslationDescriptionPastTense = "Read {0} book",
                TranslationDescriptionPresentTense = "Read {0} book"
            };
            await _pointTranslationRepository.AddSaveAsync(userId, pointTranslation);

            // required for a user to be an administrator
            var adminRole = await _roleRepository.AddSaveAsync(userId, new Model.Role
            {
                Name = "System Administrator"
            });

            // add code to make first user system administrator
            await _authorizationCodeRepository.AddSaveAsync(userId, new AuthorizationCode
            {
                Code = _config[ConfigurationKey.InitialAuthorizationCode],
                Description = "Initial code to grant system administrator status.",
                IsSingleUse = true,
                RoleId = adminRole.Id,
                SiteId = site.Id
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
            await _challengeRepository.SaveAsync();
        }

        public async Task InsertSampleData(ClaimsPrincipal currentUser)
        {
            int userId = GetId(currentUser, ClaimType.UserId);
            var user = await _userRepository.GetByIdAsync(userId);

            //insert sample data
            var challenge = new Model.Challenge
            {
                SiteId = user.SiteId,
                RelatedSystemId = user.SystemId,
                Name = "Get Along",
                Description = "This is a challenge encourging you to get along with others!",
                IsActive = false,
                IsDeleted = false,
                PointsAwarded = 10,
                TasksToComplete = 2,
                RelatedBranchId = user.BranchId
            };

            challenge = await _challengeRepository.AddSaveAsync(userId, challenge);

            int positionCounter = 1;
            await _challengeTaskRepository.AddSaveAsync(userId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Be excellent to each other",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });
            await _challengeTaskRepository.AddSaveAsync(userId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Party on, dudes!",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });

            challenge = new Model.Challenge
            {
                SiteId = user.SiteId,
                RelatedSystemId = user.SystemId,
                Name = "Science Fiction reading list",
                Description = "Read some excellent science fiction!",
                IsActive = false,
                IsDeleted = false,
                PointsAwarded = 10,
                TasksToComplete = 2,
                RelatedBranchId = user.BranchId
            };

            challenge = await _challengeRepository.AddSaveAsync(userId, challenge);
            positionCounter = 0;

            await _challengeTaskRepository.AddSaveAsync(userId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death",
                Author = "Kurt Vonnegut",
                Isbn = "978-0385333849",
                ChallengeTaskType = Model.ChallengeTaskType.Book,
                Position = positionCounter++
            });

            await _challengeTaskRepository.AddSaveAsync(userId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Stories of Your Life and Others",
                Author = "Ted Chiang",
                Isbn = "978-1101972120",
                ChallengeTaskType = Model.ChallengeTaskType.Book,
                Position = positionCounter++
            });

            await _challengeTaskRepository.AddSaveAsync(userId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Have Space Suit - Will Travel",
                Author = "Robert A. Heinlein",
                Isbn = "978-1416505495",
                ChallengeTaskType = Model.ChallengeTaskType.Book,
                Position = positionCounter++
            });

            var userCheck = await _userRepository.GetByUsernameAsync("aweasley");
            if (userCheck == null)
            {
                // add some family users
                var newUser = new Model.User
                {
                    SiteId = user.SiteId,
                    BranchId = user.BranchId,
                    SystemId = user.SystemId,
                    ProgramId = user.ProgramId,
                    FirstName = "Arthur",
                    LastName = "Weasley",
                    Username = "aweasley"
                };

                var arthur = await _userRepository.AddSaveAsync(userId, newUser);

                await _mailRepository.AddSaveAsync(userId, new Mail
                {
                    Body = "Thanks for joining our reading program, Arthur. You're the best!",
                    ToUserId = arthur.Id,
                    IsNew = true,
                    Subject = "Welcome to the program!",
                    SiteId = arthur.SiteId
                });

                await _activityService.LogActivityAsync(currentUser, arthur.Id, 1);
                await _activityService.AddBook(currentUser, arthur.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death"
                });
                await _activityService.LogActivityAsync(currentUser, arthur.Id, 1);
                await _activityService.AddBook(currentUser, arthur.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Breakfast of Champions, or Goodbye Blue Monday"
                });

                newUser.FirstName = "Molly";
                newUser.Username = null;
                newUser.HouseholdHeadUserId = arthur.Id;
                var molly = await _userRepository.AddSaveAsync(userId, newUser);

                await _activityService.LogActivityAsync(currentUser, molly.Id, 1);
                await _activityService.AddBook(currentUser, molly.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Cat's Cradle"
                });

                newUser.FirstName = "Bill";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Charlie";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Fred";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Ron";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "George";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Ginny";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Percy";
                await _userRepository.AddAsync(userId, newUser);
                await _userRepository.SaveAsync();
            }
        }
    }
}
