using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.CommandLine.Base;
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
            DataGenerator.Activity activityDataGenerator,
            DataGenerator.DateTime dateTimeDataGenerator,
            ActivityService activityService)
            : base(serviceFacade)
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
                    "Suppress status while creating users",
                    CommandOptionType.NoValue);

                _.OnExecute(async () =>
                {
                    bool quiet = displayStatusOption.HasValue()
                        && displayStatusOption.Value().Equals("on", StringComparison.CurrentCultureIgnoreCase);

                    if (createRandomOption.HasValue())
                    {
                        if (!int.TryParse(createRandomOption.Value(), out int howMany))
                        {
                            throw new ArgumentException("Error: <count> must be a number random activity items to enter.");
                        }
                        return await EnterActivity(howMany, quiet);
                    }
                    else
                    {
                        _.ShowHelp();
                        return 2;
                    }
                });
            }, throwOnUnexpectedArg: true);

            async Task<int> EnterActivity(int howMany, bool quiet)
            {
                int inserted = 0;

                var activities = await _activityDataGenerator.Generate(Site, howMany);

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
                        if (!string.IsNullOrEmpty(activity.SecretCode))
                        {
                            await _activityService
                                .LogSecretCodeAsync(activity.User.Id, activity.SecretCode);
                        }
                        else
                        {
                            await _activityService
                                .LogActivityAsync(activity.User.Id, activity.ActivityAmount);
                        }
                        inserted++;
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

                return inserted == howMany ? 0 : 1;
            }
        }
    }
}
