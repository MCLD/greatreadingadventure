using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class SampleDataService : BaseService<SampleDataService>
    {
        private readonly IChallengeRepository _challengeRepository;
        private readonly IChallengeTaskRepository _challengeTaskRepository;
        private readonly IMailRepository _mailRepository;
        private readonly IProgramRepository _programRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;
        private readonly ActivityService _activityService;
        private readonly SchoolService _schoolService;
        public SampleDataService(ILogger<SampleDataService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IChallengeRepository challengeRepository,
            IChallengeTaskRepository challengeTaskRepository,
            IMailRepository mailRepository,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            IUserRepository userRepository,
            ActivityService activityService,
            SchoolService schoolService) : base(logger, dateTimeProvider)
        {
            _challengeRepository = Require.IsNotNull(challengeRepository,
                nameof(challengeRepository));
            _challengeTaskRepository = Require.IsNotNull(challengeTaskRepository,
                nameof(challengeTaskRepository));
            _mailRepository = Require.IsNotNull(mailRepository, nameof(mailRepository));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
        }

        private int ProgramIdSearch(IEnumerable<Program> programs, string nameContains)
        {
            if (programs.Count() == 1)
            {
                return programs.First().Id;
            }
            var matches = programs
                .Where(_ => _.Name.Contains(nameContains))
                .FirstOrDefault();
            if (matches == null)
            {
                return programs.First().Id;
            }
            else
            {
                return matches.Id;
            }
        }

        public async Task InsertSampleData(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            var programs = await _programRepository.GetAllAsync(user.SiteId);
            int prereaderProgramId = ProgramIdSearch(programs, "Prereader");
            int kidsProgramId = ProgramIdSearch(programs, "Kid");
            int teensProgramId = ProgramIdSearch(programs, "Teen");
            int adultProgramId = ProgramIdSearch(programs, "Adult");

            //insert sample data
            var challenge = new Challenge
            {
                SiteId = user.SiteId,
                RelatedSystemId = user.SystemId,
                BadgeId = null,
                Name = "Get Along",
                Description = "This is a challenge encourging you to get along with others!",
                IsActive = false,
                IsDeleted = false,
                IsValid = true,
                PointsAwarded = 10,
                TasksToComplete = 2,
                RelatedBranchId = user.BranchId
            };

            challenge = await _challengeRepository.AddSaveAsync(userId, challenge);

            int positionCounter = 1;
            await _challengeTaskRepository.AddSaveAsync(userId, new ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Be excellent to each other",
                ChallengeTaskType = ChallengeTaskType.Action,
                Position = positionCounter++
            });
            await _challengeTaskRepository.AddSaveAsync(userId, new ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Party on, dudes!",
                ChallengeTaskType = ChallengeTaskType.Action,
                Position = positionCounter++
            });

            challenge = new Challenge
            {
                SiteId = user.SiteId,
                BadgeId = null,
                RelatedSystemId = user.SystemId,
                Name = "Science Fiction reading list",
                Description = "Read some excellent science fiction!",
                IsActive = false,
                IsDeleted = false,
                IsValid = true,
                PointsAwarded = 10,
                TasksToComplete = 2,
                RelatedBranchId = user.BranchId
            };

            challenge = await _challengeRepository.AddSaveAsync(userId, challenge);
            positionCounter = 0;

            await _challengeTaskRepository.AddSaveAsync(userId, new ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death",
                Author = "Kurt Vonnegut",
                Isbn = "978-0385333849",
                ChallengeTaskType = ChallengeTaskType.Book,
                Position = positionCounter++
            });

            await _challengeTaskRepository.AddSaveAsync(userId, new ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Stories of Your Life and Others",
                Author = "Ted Chiang",
                Isbn = "978-1101972120",
                ChallengeTaskType = ChallengeTaskType.Book,
                Position = positionCounter++
            });

            await _challengeTaskRepository.AddSaveAsync(userId, new ChallengeTask
            {
                ChallengeId = challenge.Id,
                Title = "Have Space Suit - Will Travel",
                Author = "Robert A. Heinlein",
                Isbn = "978-1416505495",
                ChallengeTaskType = ChallengeTaskType.Book,
                Position = positionCounter++
            });

            var district = await _schoolService.AddDistrict(new SchoolDistrict()
            {
                Name = "International Confederation of Wizards"
            });
            var typeInstitute = await _schoolService.AddSchoolType("Institute");
            var typeAcademy = await _schoolService.AddSchoolType("Academy");
            var typeSchool = await _schoolService.AddSchoolType("School of Witchcraft and Wizardry");

            var schoolHogwarts = await _schoolService.AddSchool("Hogwarts", district.Id, typeSchool.Id);
            await _schoolService.AddSchool("Ilvermorny", district.Id, typeSchool.Id);
            var schoolBeauxbatons = await _schoolService.AddSchool("Beauxbatons", district.Id, typeAcademy.Id);
            var schoolDurmstrang = await _schoolService.AddSchool("Durmstrang", district.Id, typeInstitute.Id);

            var userCheck = await _userRepository.GetByUsernameAsync("aweasley");
            if (userCheck == null)
            {
                // add some family users
                var newUser = new User
                {
                    SiteId = user.SiteId,
                    BranchId = user.BranchId,
                    SystemId = user.SystemId,
                    ProgramId = adultProgramId,
                    FirstName = "Arthur",
                    LastName = "Weasley",
                    Username = "aweasley"
                };

                var arthur = await _userRepository.AddSaveAsync(userId, newUser);
                await _userRepository.SetUserPasswordAsync(userId, arthur.Id, "123123");

                await _mailRepository.AddSaveAsync(userId, new Mail
                {
                    Body = "Thanks for joining our reading program, Arthur. You're the best!",
                    ToUserId = arthur.Id,
                    IsNew = true,
                    Subject = "Welcome to the program!",
                    SiteId = arthur.SiteId
                });

                await _activityService.LogActivityAsync(arthur.Id, 1);
                await _activityService.AddBookAsync(arthur.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Slaughterhouse-Five, or The Children's Crusade: A Duty-Dance with Death"
                });
                await _activityService.LogActivityAsync(arthur.Id, 1);
                await _activityService.AddBookAsync(arthur.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Breakfast of Champions, or Goodbye Blue Monday"
                });

                newUser.FirstName = "Molly";
                newUser.Username = null;
                newUser.HouseholdHeadUserId = arthur.Id;
                var molly = await _userRepository.AddSaveAsync(userId, newUser);

                await _activityService.LogActivityAsync(molly.Id, 1);
                await _activityService.AddBookAsync(molly.Id, new Book
                {
                    Author = "Kurt Vonnegut",
                    Title = "Cat's Cradle"
                });

                newUser.FirstName = "Bill";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Charlie";
                newUser.SchoolId = schoolHogwarts.Id;
                newUser.ProgramId = teensProgramId;
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Fred";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "George";
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Percy";
                newUser.ProgramId = adultProgramId;
                newUser.SchoolId = default(int?);
                await _userRepository.AddAsync(userId, newUser);
                await _userRepository.SaveAsync();

                newUser.FirstName = "Ron";
                newUser.HouseholdHeadUserId = null;
                var ron = await _userRepository.AddSaveAsync(userId, newUser);

                newUser.FirstName = "Hermione";
                newUser.LastName = "Granger";
                newUser.HouseholdHeadUserId = ron.Id;
                var hermione = await _userRepository.AddSaveAsync(userId, newUser);

                await _activityService.LogActivityAsync(ron.Id, 1);
                await _activityService.LogActivityAsync(hermione.Id, 1);
                await _activityService.LogActivityAsync(hermione.Id, 1);
                await _activityService.LogActivityAsync(hermione.Id, 1);
                await _activityService.LogActivityAsync(hermione.Id, 1);
                await _activityService.LogActivityAsync(hermione.Id, 1);

                newUser.ProgramId = kidsProgramId;
                newUser.FirstName = "Rose";
                newUser.LastName = "Granger-Weasley";
                newUser.SchoolId = schoolBeauxbatons.Id;
                await _userRepository.AddAsync(userId, newUser);
                newUser.ProgramId = prereaderProgramId;
                newUser.FirstName = "Hugo";
                await _userRepository.AddAsync(userId, newUser);
                await _userRepository.SaveAsync();

                newUser.FirstName = "Harry";
                newUser.LastName = "Potter";
                newUser.ProgramId = adultProgramId;
                newUser.HouseholdHeadUserId = null;
                newUser.SchoolId = default(int?);
                var harry = await _userRepository.AddSaveAsync(userId, newUser);

                newUser.FirstName = "Ginevra";
                newUser.HouseholdHeadUserId = harry.Id;
                var ginny = await _userRepository.AddSaveAsync(userId, newUser);

                await _activityService.LogActivityAsync(harry.Id, 1);
                await _activityService.LogActivityAsync(harry.Id, 1);
                await _activityService.LogActivityAsync(ginny.Id, 1);

                newUser.FirstName = "James";
                newUser.ProgramId = teensProgramId;
                newUser.SchoolId = schoolDurmstrang.Id;
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Albus";
                newUser.ProgramId = kidsProgramId;
                newUser.SchoolId = schoolHogwarts.Id;
                await _userRepository.AddAsync(userId, newUser);
                newUser.FirstName = "Lily";
                newUser.SchoolId = default(int?);
                await _userRepository.AddAsync(userId, newUser);
                await _userRepository.SaveAsync();
            }
        }
    }
}
