using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.CommandLine.Base;
using GRA.CommandLine.FakeWeb;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Commands
{
    internal class UserCommand : BaseCommand
    {
        private readonly DataGenerator.DateTime _dateTimeDataGenerator;
        private readonly DataGenerator.User _userDataGenerator;
        private readonly ReportService _reportService;
        private const int FamilyRandomIndicator = -1;
        private const int GroupRandomIndicator = -2;
        public UserCommand(ServiceFacade facade,
            ConfigureUserSite configureUserSite,
            DataGenerator.DateTime dateTimeDataGenerator,
            DataGenerator.User userDataGenerator,
            ReportService reportService) : base(facade, configureUserSite)
        {
            _dateTimeDataGenerator = dateTimeDataGenerator
                ?? throw new ArgumentNullException(nameof(dateTimeDataGenerator));
            _userDataGenerator = userDataGenerator
                ?? throw new ArgumentNullException(nameof(userDataGenerator));
            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));

            _facade.App.Command("user", _ =>
            {
                _.Description = "Create, read, update, or delete users";
                _.HelpOption("-?|-h|--help");

                var repeatOption = _.Option("-r|--repeat <count>",
                    "Repeat <count> times",
                    CommandOptionType.SingleValue);

                var createRandomOption = _.Option("-cr|--createrandom <count>",
                    "Create <count> random users",
                    CommandOptionType.SingleValue);

                var displayStatusOption = _.Option("-q|--quiet",
                    "Suppress status while creating users",
                    CommandOptionType.NoValue);

                var householdOption = _.Option("-hh|--household",
                    "Also create households when inserting random users",
                    CommandOptionType.NoValue);

                var countCommand = _.Command("count", _c =>
                {
                    _c.Description = "Get a total number of users in a site.";
                    _c.HelpOption("-?|-h|--help");

                    _c.OnExecute(async () =>
                    {
                        await EnsureUserAndSiteLoaded();
                        return await DisplayUserCount();
                    });
                });

                _.OnExecute(async () =>
                {
                    bool quiet = displayStatusOption.HasValue()
                        && displayStatusOption.Value().Equals("on", StringComparison.CurrentCultureIgnoreCase);

                    bool household = householdOption.HasValue()
                        && householdOption.Value().Equals("on", StringComparison.CurrentCultureIgnoreCase);

                    int repeat = 1;

                    if(repeatOption.HasValue())
                    {
                        if (!int.TryParse(repeatOption.Value(), out repeat))
                        {
                            throw new ArgumentException("Error: <count> must be a number of times to repeat.");
                        }
                    }

                    if (createRandomOption.HasValue())
                    {
                        if (!int.TryParse(createRandomOption.Value(), out int howMany))
                        {
                            throw new ArgumentException("Error: <count> must be a number of users to create.");
                        }
                        int result = 0;
                        while(repeat >= 1)
                        {
                            var thisResult = await CreateUsers(howMany, household, quiet);
                            result = Math.Max(result, thisResult);
                            repeat--;
                            Console.WriteLine();
                            if (repeat > 0)
                            {
                                Console.WriteLine($"Repeating {repeat} more time(s).");
                            }
                        }
                        return result;
                    }
                    else
                    {
                        _.ShowHelp();
                        return 2;
                    }
                });
            }, throwOnUnexpectedArg: true);
        }

        private async Task<int> DisplayUserCount()
        {
            var users = await _facade.UserService.GetPaginatedUserListAsync(new UserFilter());
            var report = await _reportService
                .GetCurrentStatsAsync(new Domain.Model.ReportCriterion());
            Console.WriteLine($"Total users in {Site.Name}: {users.Count}; achievers: {report.Achievers}");
            return 0;
        }

        private async Task<int> CreateUsers(int howMany, bool household, bool quiet)
        {
            int created = 0;

            int[] familyOptions = { 1, 2, 3, 4, 5, FamilyRandomIndicator, GroupRandomIndicator };
            float[] familyWeights = { 0.25F, 0.35F, 0.21F, 0.10F, 0.04F, 0.047F, 0.003F };

            var issues = new List<string>();

            // make the participants
            var users = await _userDataGenerator.Generate(Site.Id, howMany);

            var minDateTime = DateTime.MaxValue;
            var maxDateTime = DateTime.MinValue;

            if (!quiet)
            {
                Console.Write($"Inserting {howMany} users... ");
            }

            ProgressBar progress = quiet ? null : new ProgressBar();
            try
            {
                var rand = new Bogus.Randomizer();
                int familyMembers = 0;
                Domain.Model.User parent = null;

                // insert the participants
                foreach (var user in users)
                {
                    bool currentUserParent = false;

                    // set an appropriate random date and time for insertion
                    var setDateTime = _dateTimeDataGenerator.SetRandom(Site);

                    if (setDateTime < minDateTime)
                    {
                        minDateTime = setDateTime;
                    }
                    if (setDateTime > maxDateTime)
                    {
                        maxDateTime = setDateTime;
                    }

                    if (familyMembers > 0)
                    {
                        // we are processing family members
                        user.User.LastName = parent.LastName;
                        if (rand.Int(1, 100) > 5)
                        {
                            user.User.Username = null;
                        }

                        // insert the family member
                        try
                        {
                            await _facade
                                .UserService
                                .AddHouseholdMemberAsync(parent.Id, user.User);
                            created++;
                        }
                        catch (GraException gex)
                        {
                            issues.Add($"Household username: {user.User.Username} - {gex.Message}");
                        }
                        familyMembers--;
                    }
                    else
                    {
                        // not processing family members, should this person be a head of household?
                        if (household && rand.Int(1, 100) <= 31)
                        {
                            currentUserParent = true;

                            familyMembers = rand.WeightedRandom<int>(familyOptions, familyWeights);
                            if (familyMembers == FamilyRandomIndicator)
                            {
                                familyMembers = rand.Int(6, 10);
                            }
                            else if (familyMembers == GroupRandomIndicator)
                            {
                                familyMembers = rand.Int(11, 100);
                            }
                        }

                        // insert the created user
                        try
                        {
                            var inserted = await _facade
                                .UserService
                                .RegisterUserAsync(user.User, user.Password);
                            if (currentUserParent)
                            {
                                parent = inserted;
                            }
                            created++;
                        }
                        catch (Exception ex)
                        {
                            issues.Add($"Username: {user.User.Username} - {ex.Message}");
                        }
                    }

                    if (progress != null)
                    {
                        progress.Report((double)created / howMany);
                    }
                }
            }
            finally
            {
                if (progress != null)
                {
                    progress.Dispose();
                }
            }

            Console.WriteLine($"Created {created} random users in {Site.Name}.");
            Console.WriteLine($"Users registered between {minDateTime} and {maxDateTime}.");

            if (issues.Count > 0)
            {
                Console.WriteLine("Some issues were encountered:");
                foreach (string issue in issues)
                {
                    Console.WriteLine($"- {issue}");
                }
            }

            await DisplayUserCount();
            return howMany == created ? 0 : 1;
        }
    }
}
