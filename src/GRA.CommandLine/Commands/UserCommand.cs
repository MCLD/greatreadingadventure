using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.CommandLine.Base;
using GRA.Controllers;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Commands
{
    internal class UserCommand : BaseCommand
    {
        public UserCommand(ServiceFacade facade) : base(facade)
        {
            _facade.App.Command("user", _ =>
            {
                _.Description = "Create, read, update, or delete users";
                _.HelpOption("-?|-h|--help");

                var countCommand = _.Command("count", _c =>
                {
                    _c.Description = "Get a total number of users in a site.";
                    _c.HelpOption("-?|-h|--help");

                    _c.OnExecute(async () =>
                    {
                        await LoadUserAndSiteAsync();
                        var users = await _facade.UserService.GetPaginatedUserListAsync(new UserFilter());

                        Console.WriteLine($"Total users: {users.Count}");

                        return 0;
                    });
                });

                _.OnExecute(() =>
                {
                    return 0;
                });
            }, throwOnUnexpectedArg: true);
        }
    }
}
