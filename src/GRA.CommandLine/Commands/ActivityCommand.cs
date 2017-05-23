using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.CommandLine.Base;
using GRA.CommandLine.FakeWeb;
using GRA.Domain.Service;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Commands
{
    class ActivityCommand : BaseCommand
    {
        private readonly DataGenerator.Activity _activityDataGenerator;
        private readonly DataGenerator.DateTime _dateTimeDataGenerator;
        private readonly ActivityService _activityService;
        public ActivityCommand(ServiceFacade serviceFacade,
            ConfigureUserSite configureUserSite,
            DataGenerator.Activity activityDataGenerator,
            DataGenerator.DateTime dateTimeDataGenerator,
            ActivityService activityService)
            : base(serviceFacade, configureUserSite)
        {
            _activityDataGenerator = activityDataGenerator
                ?? throw new ArgumentNullException(nameof(activityDataGenerator));
            _dateTimeDataGenerator = dateTimeDataGenerator
                ?? throw new ArgumentNullException(nameof(dateTimeDataGenerator));
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));

            _facade.App.Command("activity", _ =>
            {
                _.Description = "Create, read, update, or delete activity";
                _.HelpOption("-?|-h|--help");

                var createRandomOption = _.Option("-er|--enterrandom <count>",
                    "Enter <count> random activity items",
                    CommandOptionType.SingleValue);

                var displayStatusOption = _.Option("-q|--quiet",
                    "Suppress status while entering activity items",
                    CommandOptionType.NoValue);

                var challengeStatusOption = _.Option("-c|--challenge",
                    "Include random challenge tasks when entering random activity items",
                    CommandOptionType.NoValue);

                var challengePercentOption = _.Option("-cp|--challengepercent <percentage>",
                    "Specify what percentage of activities should be challenges. Defaults to 30.",
                    CommandOptionType.SingleValue);

                _.OnExecute(async () =>
                {
                    bool quiet = displayStatusOption.HasValue()
                        && displayStatusOption.Value().Equals("on", StringComparison.CurrentCultureIgnoreCase);

                    bool challenges = challengeStatusOption.HasValue()
                        && challengeStatusOption.Value().Equals("on", StringComparison.CurrentCultureIgnoreCase);

                    int challengePercent = challenges ? 30 : 0;
                    if(challenges && challengePercentOption.HasValue())
                    {
                        if(!(int.TryParse(challengePercentOption.Value(), out challengePercent)
                            && challengePercent > 0
                            && challengePercent < 101))
                        {
                            throw new ArgumentException("Error: <percentage> must be a number between 1 and 100.");
                        }
                    }

                    if (createRandomOption.HasValue())
                    {
                        if (!int.TryParse(createRandomOption.Value(), out int howMany))
                        {
                            throw new ArgumentException("Error: <count> must be a number random activity items to enter.");
                        }
                        return await EnterActivity(howMany, challengePercent, quiet);
                    }
                    else
                    {
                        _.ShowHelp();
                        return 2;
                    }
                });
            }, throwOnUnexpectedArg: true);

            async Task<int> EnterActivity(int howMany, int challengePercent, bool quiet)
            {
                int inserted = 0;
                var issues = new List<string>();

                var activities 
                    = await _activityDataGenerator.Generate(Site, howMany, challengePercent, quiet);

                if (!quiet)
                {
                    Console.Write($"Inserting {howMany} activity items... ");
                }

                ProgressBar progress = quiet ? null : new ProgressBar();
                try
                {
                    foreach (var activity in activities)
                    {
                        _dateTimeDataGenerator.SetRandom(Site, activity.User);
                        try
                        {
                            await _configureUserSite.Lookup(activity.User.Id);
                            _activityService.ClearCachedUserContext();
                            switch (activity.ActivityType)
                            {
                                case DataGenerator.ActivityType.SecretCode:
                                    await _activityService
                                        .LogSecretCodeAsync(activity.User.Id, activity.SecretCode);
                                    break;

                                case DataGenerator.ActivityType.ChallengeTasks:
                                    await _activityService
                                        .UpdateChallengeTasksAsync(activity.ChallengeId, activity.ChallengeTasks);
                                    break;

                                default:
                                case DataGenerator.ActivityType.Default:
                                    await _activityService
                                        .LogActivityAsync(activity.User.Id, activity.ActivityAmount);
                                    break;
                            }
                            inserted++;
                        }
                        catch (GraException gex)
                        {
                            switch (activity.ActivityType)
                            {
                                case DataGenerator.ActivityType.ChallengeTasks:
                                    issues.Add($"Problem logging challenge tasks for {activity.User.Id}: {gex.Message}");
                                    break;
                                case DataGenerator.ActivityType.SecretCode:
                                    issues.Add($"Problem logging code {activity.SecretCode} for {activity.User.Id}: {gex.Message}");
                                    break;
                                default:
                                case DataGenerator.ActivityType.Default:
                                    issues.Add($"Problem logging {activity.ActivityAmount} for {activity.User.Id}: {gex.Message}");
                                    break;
                            }
                        }
                        if (progress != null)
                        {
                            progress.Report((double)inserted / howMany);
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

                Console.WriteLine($"Inserted {inserted} random activity items in {Site.Name}.");

                if (issues.Count > 0)
                {
                    Console.WriteLine("Some issues were encountered:");
                    foreach (string issue in issues)
                    {
                        Console.WriteLine($"- {issue}");
                    }
                }

                return inserted == howMany ? 0 : 1;
            }
        }
    }
}
