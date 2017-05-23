using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using GRA.CommandLine.FakeWeb;
using GRA.Controllers;
using GRA.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Base
{
    public abstract class BaseCommand : CommandLineApplication
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
