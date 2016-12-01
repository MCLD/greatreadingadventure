using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class ConfigurationService : Abstract.BaseService<ConfigurationService>
    {
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
        public ConfigurationService(ILogger<ConfigurationService> logger,
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
            IPointTranslationRepository pointTranslationRepository) : base(logger)
        {
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
        }

        public async Task<bool> NeedsInitialSetupAsync()
        {
            var firstSite = await _siteRepository.GetAllAsync();
            return firstSite.Count() == 0 || firstSite.First().CreatedBy == -1;
        }

        public async Task<Model.User> InitialSetupAsync(Model.User adminUser, string password)
        {
            var allSites = await _siteRepository.GetAllAsync();

            var site = allSites.First();

            var system = new Model.System
            {
                SiteId = site.Id,
                Name = "Maricopa County Library District"
            };
            system = await _systemRepository.AddSaveAsync(-1, system);

            var branch = new Model.Branch
            {
                SiteId = site.Id,
                SystemId = system.Id,
                Name = "Admin",
                Address = "2700 N. Central Ave Ste 700, 85004",
                Telephone = "602-652-3064",
                Url = "http://mcldaz.org/"
            };
            branch = await _branchRepository.AddSaveAsync(-1, branch);

            var program = new Model.Program
            {
                SiteId = site.Id,
                AchieverPointAmount = 1000,
                Name = "Winter Reading Program",
            };
            program = await _programRepository.AddSaveAsync(-1, program);

            adminUser.BranchId = branch.Id;
            adminUser.ProgramId = program.Id;
            adminUser.SiteId = site.Id;
            adminUser.SystemId = system.Id;
            adminUser.CanBeDeleted = false;
            var user = await _userRepository.AddSaveAsync(0, adminUser);
            await _userRepository.SetUserPasswordAsync(user.Id, user.Id, password);

            int creatorUserId = user.Id;

            site.CreatedBy = creatorUserId;
            site = await _siteRepository.UpdateSaveAsync(creatorUserId, site);

            system.CreatedBy = creatorUserId;
            system = await _systemRepository.UpdateSaveAsync(creatorUserId, system);

            branch.CreatedBy = creatorUserId;
            branch = await _branchRepository.UpdateSaveAsync(creatorUserId, branch);

            program.CreatedBy = creatorUserId;
            program = await _programRepository.UpdateSaveAsync(creatorUserId, program);

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
            await _pointTranslationRepository.AddSaveAsync(creatorUserId, pointTranslation);

            var adminRole = await _roleRepository.AddSaveAsync(creatorUserId, new Model.Role
            {
                Name = "System Administrator"
            });

            await _userRepository.AddRoleAsync(creatorUserId, user.Id, adminRole.Id);

            foreach (var value in Enum.GetValues(typeof(Model.Permission)))
            {
                await _roleRepository.AddPermissionAsync(creatorUserId, value.ToString());
            }
            await _roleRepository.SaveAsync();

            foreach (var value in Enum.GetValues(typeof(Model.Permission)))
            {
                await _roleRepository.AddPermissionToRoleAsync(creatorUserId,
                    adminRole.Id,
                    value.ToString());
            }
            await _roleRepository.SaveAsync();

            foreach (var value in Enum.GetValues(typeof(Model.ChallengeTaskType)))
            {
                await _challengeTaskRepository.AddChallengeTaskTypeAsync(creatorUserId,
                    value.ToString());
            }
            await _challengeRepository.SaveAsync();

            // sample challenge

            var challenge = new Model.Challenge
            {
                SiteId = site.Id,
                RelatedSystemId = system.Id,
                Name = "Test challenge",
                Description = "This is a test challenge!",
                IsActive = false,
                IsDeleted = false,
                PointsAwarded = 10,
                TasksToComplete = 2,
                RelatedBranchId = branch.Id
            };

            challenge = await _challengeRepository.AddSaveAsync(creatorUserId, challenge);

            int positionCounter = 1;
            await _challengeTaskRepository.AddSaveAsync(creatorUserId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Be excellent to each other",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });
            await _challengeTaskRepository.AddSaveAsync(creatorUserId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Party on, dudes!",
                ChallengeTaskType = Model.ChallengeTaskType.Action,
                Position = positionCounter++
            });
            await _challengeTaskRepository.AddSaveAsync(creatorUserId, new Model.ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death",
                Author = "Kurt Vonnegut",
                Isbn = "978-0385333849",
                ChallengeTaskType = Model.ChallengeTaskType.Book,
                Position = positionCounter++
            });

            // add a book for the admin

            await _bookRepository.AddSaveForUserAsync(creatorUserId, creatorUserId, new Model.Book
            {
                Author = "Kurt Vonnegut",
                Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death"
            });
            await _bookRepository.AddSaveForUserAsync(creatorUserId, creatorUserId, new Model.Book
            {
                Author = "Kurt Vonnegut",
                Title = "Breakfast of Champions, or Goodbye Blue Monday"
            });
            await _bookRepository.AddSaveForUserAsync(creatorUserId, creatorUserId, new Model.Book
            {
                Author = "Kurt Vonnegut",
                Title = "Cat's Cradle"
            });


            // add a welcome message to the admin

            await _mailRepository.AddSaveAsync(creatorUserId, new Model.Mail
            {
                Body = "Your administrative account has been created successfully!",
                FromUserId = creatorUserId,
                Subject = $"Welcome to {site.Name}"
            });

            // add some family users

            var newUser = new Model.User
            {
                SiteId = site.Id,
                BranchId = branch.Id,
                SystemId = system.Id,
                ProgramId = program.Id,
                FirstName = "Arthur",
                LastName = "Weasley",
                Username = "aweasley"
            };

            var arthur = await _userRepository.AddSaveAsync(creatorUserId, newUser);

            await _mailRepository.AddSaveAsync(creatorUserId, new Model.Mail
            {
                Body = "Your account has been created successfully!",
                FromUserId = arthur.Id,
                Subject = $"Welcome to {site.Name}!"
            });

            newUser.FirstName = "Molly";
            newUser.Username = null;
            newUser.HouseholdHeadUserId = arthur.Id;
            await _userRepository.AddAsync(creatorUserId, newUser);

            newUser.FirstName = "Bill";
            await _userRepository.AddAsync(creatorUserId, newUser);
            newUser.FirstName = "Charlie";
            await _userRepository.AddAsync(creatorUserId, newUser);
            newUser.FirstName = "Fred";
            await _userRepository.AddAsync(creatorUserId, newUser);
            newUser.FirstName = "Ron";
            await _userRepository.AddAsync(creatorUserId, newUser);
            newUser.FirstName = "George";
            await _userRepository.AddAsync(creatorUserId, newUser);
            newUser.FirstName = "Ginny";
            await _userRepository.AddAsync(creatorUserId, newUser);
            newUser.FirstName = "Percy";
            await _userRepository.AddAsync(creatorUserId, newUser);
            await _userRepository.SaveAsync();

            return user;
        }
    }
}
