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
        private readonly UserService _userService;
        public Activity(TriggerService triggerService,
            UserService userService)
        {
            _triggerService = triggerService
                ?? throw new ArgumentNullException(nameof(triggerService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IEnumerable<GeneratedActivity>>
            Generate(Site site, int count)
        {
            var userList = await _userService.GetPaginatedUserListAsync(new UserFilter());
            var triggerList = await _triggerService.GetPaginatedListAsync(new BaseFilter());

            var activities = new List<GeneratedActivity>();

            var rand = new Bogus.Randomizer();

            for (int i = 0; i < count; i++)
            {
                var randomUser = (await _userService.GetPaginatedUserListAsync(new UserFilter
                {
                    SiteId = site.Id,
                    Skip = rand.Int(0, userList.Count),
                    Take = 1
                })).Data.First();

                var act = new GeneratedActivity
                {
                    User = randomUser,
                };

                if (rand.Int(1, 100) <= -1)
                {
                    // TODO fix secret code activity entry
                    var randomTrigger = (await _triggerService.GetPaginatedListAsync(new BaseFilter
                    {
                        SiteId = site.Id,
                        Skip = rand.Int(0, triggerList.Count),
                        Take = 1
                    })).Data.First();

                    act.SecretCode = randomTrigger.SecretCode;
                }
                else
                {
                    int choice = rand.Int(1, 100);
                    if (choice <= 5)
                    {
                        act.ActivityAmount = rand.Int(1, 500);
                    }
                    else if (choice <= 15)
                    {
                        act.ActivityAmount = rand.Int(1, 120);
                    }
                    else
                    {
                        act.ActivityAmount = rand.Int(1, 60);
                    }
                }

                activities.Add(act);
            }

            return activities;
        }

    }
}
