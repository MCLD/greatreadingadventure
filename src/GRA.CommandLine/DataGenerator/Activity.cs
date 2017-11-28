using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Model;
using Microsoft.Extensions.Logging;
using GRA.CommandLine.FakeWeb;

namespace GRA.CommandLine.DataGenerator
{
    internal class Activity
    {
        private readonly ILogger<Activity> _logger;
        private readonly ConfigureUserSite _configureUserSite;
        private readonly TriggerService _triggerService;
        private readonly ChallengeService _challengeService;
        private readonly UserService _userService;
        public Activity(ILogger<Activity> logger,
            ConfigureUserSite configureUserSite,
            TriggerService triggerService,
            ChallengeService challengeService,
            UserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configureUserSite = configureUserSite
                ?? throw new ArgumentNullException(nameof(configureUserSite));
            _triggerService = triggerService
                ?? throw new ArgumentNullException(nameof(triggerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _challengeService = challengeService
                ?? throw new ArgumentNullException(nameof(challengeService));
        }

        public async Task<IEnumerable<GeneratedActivity>>
            Generate(Site site, int count, int challengePercent, int codePercent, bool quiet)
        {
            int[] minuteCeilings = { 60, 120, 500 };
            float[] minuteCeilingDistribution = { 0.85F, 0.1F, 0.05F };

            var userList = await _userService.GetPaginatedUserListAsync(new UserFilter());
            var codeList = await _triggerService.GetPaginatedListAsync(new TriggerFilter
            {
                SecretCodesOnly = true
            });
            var challengeList = await _challengeService
                .GetPaginatedChallengeListAsync(new ChallengeFilter());

            var activities = new List<GeneratedActivity>();

            var rand = new Bogus.Randomizer();

            if (!quiet)
            {
                Console.Write($"Generating {count} activity items... ");
            }

            ProgressBar progress = quiet ? null : new ProgressBar();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    bool addActivity = false;
                    var randomUser = (await _userService.GetPaginatedUserListAsync(new UserFilter
                    {
                        SiteId = site.Id,
                        Skip = rand.Int(0, userList.Count - 1),
                        Take = 1
                    })).Data.First();

                    var act = new GeneratedActivity
                    {
                        User = randomUser,
                    };
                    if (challengePercent > 0 && rand.Int(1, 100) <= challengePercent)
                    {

                        bool isValid = false;
                        int challengeLookupCount = 0;
                        await _configureUserSite.Lookup(randomUser.Id);
                        _challengeService.ClearCachedUserContext();
                        DataWithCount<IEnumerable<Challenge>> randomChallenge = null;
                        while (!isValid)
                        {
                            challengeLookupCount++;
                            var filter = new ChallengeFilter()
                            {
                                Take = rand.Int(0, challengeList.Count - 1),
                                Skip = 1
                            };
                            randomChallenge = await _challengeService.GetPaginatedChallengeListAsync(filter);
                            if (randomChallenge.Data != null
                                && randomChallenge.Data.FirstOrDefault() != null)
                            {
                                isValid = randomChallenge.Data.First().IsValid;
                            }
                            if (challengeLookupCount > 20)
                            {
                                _logger.LogError($"Unable to find an eligible challenge for user id {randomUser.Id} after 20 tries, giving up.");
                                randomChallenge = null;
                                addActivity = false;
                                break;
                            }
                        }
                        if (randomChallenge != null)
                        {
                            var randomTasks = await _challengeService
                                .GetChallengeTasksAsync(randomChallenge.Data.First().Id);
                            var randomTask = randomTasks
                                .Skip(rand.Int(0, randomTasks.Count() - 1)).First();
                            randomTask.IsCompleted = true;
                            act.ActivityType = ActivityType.ChallengeTasks;
                            act.ChallengeId = randomChallenge.Data.First().Id;
                            act.ChallengeTasks = new List<ChallengeTask> { randomTask };
                            addActivity = true;
                        }
                    }
                    else
                    {
                        if (codePercent > 0 && rand.Int(1, 100) <= codePercent)
                        {
                            var randomCode = (await _triggerService.GetPaginatedListAsync(new TriggerFilter
                            {
                                SiteId = site.Id,
                                Skip = rand.Int(0, codeList.Count - 1),
                                Take = 1,
                                SecretCodesOnly = true
                            })).Data.First();

                            act.ActivityType = ActivityType.SecretCode;
                            act.SecretCode = randomCode.SecretCode;
                            addActivity = true;
                        }
                        else
                        {
                            act.ActivityAmount = rand.Int(1, rand
                                .WeightedRandom<int>(minuteCeilings, minuteCeilingDistribution));
                            act.ActivityType = ActivityType.Default;
                            addActivity = true;
                        }
                    }
                    if (addActivity)
                    {
                        activities.Add(act);
                    }

                    if (progress != null)
                    {
                        progress.Report((double)i / count);
                    }
                }
            }
            finally
            {
                if (progress != null)
                {
                    progress.Dispose();
                    Console.WriteLine();
                }
            }
            return activities;
        }

    }
}
