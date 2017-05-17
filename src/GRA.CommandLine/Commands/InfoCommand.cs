using System;
using GRA.CommandLine.Base;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Commands
{
    internal class InfoCommand : BaseCommand
    {
        public InfoCommand(ServiceFacade facade) : base(facade)
        {
            _facade.App.Command("info", _ =>
            {
                _.Description = "Show information about this GRA installation";
                _.HelpOption("-?|-h|--help");

                var siteIdOption = _.Option("-s|--siteid <siteid>",
                    "Which site id to look up",
                    CommandOptionType.SingleValue);

                _.OnExecute(async () =>
                {
                    int supplementalSiteId = 0;
                    if(!string.IsNullOrEmpty(siteIdOption.Value()))
                    {
                        if(!int.TryParse(siteIdOption.Value(), out supplementalSiteId))
                        {
                            throw new ArgumentException("<siteid> must be a number!");
                        }
                    }

                    var defaultId = await _facade.SiteLookupService.GetDefaultSiteIdAsync();
                    var site = await _facade.SiteLookupService.GetByIdAsync(defaultId);
                    Console.Out.WriteLine("Information about this instance:");
                    Console.Out.WriteLine($"Default site id = {defaultId}, name = {site.Name}");
                    if(!string.IsNullOrEmpty(siteIdOption.Value()))
                    {
                        var specificSite = await _facade.SiteLookupService.GetByIdAsync(supplementalSiteId);
                        if(specificSite == null)
                        {
                            Console.Out.WriteLine($"Requested site id {supplementalSiteId} not found!");
                        }
                        else
                        {
                            Console.Out.WriteLine($"Requested site id {supplementalSiteId}, name = {specificSite.Name}");
                        }
                    }
                    return 0;
                });
            }, throwOnUnexpectedArg: true);
        }
    }
}
