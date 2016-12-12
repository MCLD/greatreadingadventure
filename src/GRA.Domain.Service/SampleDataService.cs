using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;

namespace GRA.Domain.Service
{
    public class SampleDataService : BaseService<SampleDataService>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IMailRepository _mailRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;
        private readonly ActivityService _activityService;
        public SampleDataService(ILogger<SampleDataService> logger,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IMailRepository mailRepository,
            ISiteRepository siteRepository,
            IUserRepository userRepository,
            ActivityService activityService
            ) : base(logger)
        {
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
        }

        public async Task InsertSampleData(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            //insert sample data
            var challenge = new Model.Challenge
            {
                SiteId = user.SiteId,
                RelatedSystemId = user.SystemId,
                BadgeId = null,
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
                BadgeId = null,
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

                await _activityService.LogActivityAsync(arthur.Id, 1);
                await _activityService.AddBook(arthur.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death"
                });
                await _activityService.LogActivityAsync(arthur.Id, 1);
                await _activityService.AddBook(arthur.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Breakfast of Champions, or Goodbye Blue Monday"
                });

                newUser.FirstName = "Molly";
                newUser.Username = null;
                newUser.HouseholdHeadUserId = arthur.Id;
                var molly = await _userRepository.AddSaveAsync(userId, newUser);

                await _activityService.LogActivityAsync(molly.Id, 1);
                await _activityService.AddBook(molly.Id, new Book
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
