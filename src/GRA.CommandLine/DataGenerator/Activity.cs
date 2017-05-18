using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Model;

namespace GRA.CommandLine.DataGenerator
{
    internal class Activity
    {
        private readonly TriggerService _triggerService;
        private readonly ChallengeService _challengeService;
        private readonly UserService _userService;
        public Activity(TriggerService triggerService,
            ChallengeService challengeService,
            UserService userService)
        {
            _triggerService = triggerService
                ?? throw new ArgumentNullException(nameof(triggerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _challengeService = challengeService
                ?? throw new ArgumentNullException(nameof(challengeService));
        }

        public async Task<IEnumerable<GeneratedActivity>>
            Generate(Site site, int count, bool challenges, bool quiet)
        {
            int[] minuteCeilings = { 60, 120, 500 };
            float[] minuteCeilingDistribution = { 0.85F, 0.1F, 0.05F };

            var userList = await _userService.GetPaginatedUserListAsync(new UserFilter());
            var codeList = await _triggerService.GetPaginatedListAsync(new TriggerFilter
            {
                SecretCodesOnly = true
            });
            var challengeList = await _challengeService.GetPaginatedChallengeListAsync(0, 15);

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
                    if (challenges && rand.Int(1, 100) <= 30)
                    {
                        var randomChallenge = await _challengeService
                            .GetPaginatedChallengeListAsync(rand.Int(0, challengeList.Count - 1), 1);
                        var randomTasks = await _challengeService
                            .GetChallengeTasksAsync(randomChallenge.Data.First().Id);
                        var randomTask = randomTasks.Skip(rand.Int(0, randomTasks.Count() - 1)).First();
                        randomTask.IsCompleted = true;
                        act.ActivityType = ActivityType.ChallengeTasks;
                        act.ChallengeId = randomChallenge.Data.First().Id;
                        act.ChallengeTasks = new List<ChallengeTask> { randomTask };
                    }
                    else
                    {
                        if (rand.Int(1, 100) <= 20)
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
                        }
                        else
                        {
                            act.ActivityAmount = rand.Int(1, rand
                                .WeightedRandom<int>(minuteCeilings, minuteCeilingDistribution));
                            act.ActivityType = ActivityType.Default;
                        }
                    }
                    activities.Add(act);

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
