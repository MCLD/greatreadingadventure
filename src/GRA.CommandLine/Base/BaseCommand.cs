using System;
using System.Threading.Tasks;
using GRA.CommandLine.FakeWeb;
using GRA.Domain.Model;
using McMaster.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Base
{
    abstract class BaseCommand : CommandLineApplication
    {
        protected readonly ServiceFacade _facade;
        protected readonly ConfigureUserSite _configureUserSite;
        private User _user;
        private Site _site;

        protected User User
        {
            get
            {
                if (_user == null)
                {
                    var task = Task.Run(() => EnsureUserAndSiteLoaded());
                    task.Wait();
                }
                return _user;
            }
            private set
            {
                _user = value;
            }
        }

        protected Site Site
        {
            get
            {
                if (_site == null)
                {
                    var task = Task.Run(() => EnsureUserAndSiteLoaded());
                    task.Wait();
                }
                return _site;
            }
            private set
            {
                _site = value;
            }
        }

        public BaseCommand(ServiceFacade serviceFacade, ConfigureUserSite configureUserSite)
        {
            _facade = serviceFacade ?? throw new ArgumentNullException(nameof(serviceFacade));
            _configureUserSite = configureUserSite
                ?? throw new ArgumentNullException(nameof(configureUserSite));
        }

        protected async Task EnsureUserAndSiteLoaded(bool force = false)
        {
            if (!force)
            {
                if (_site != null && _user != null)
                {
                    return;
                }
            }
            var userSite = await _configureUserSite.Lookup();
            User = userSite.User;
            Site = userSite.Site;
        }
    }
}
