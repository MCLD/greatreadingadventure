using System;
using GRA.Abstract;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace GRA.CommandLine
{
    public class ServiceFacade
    {
        public CommandLineApplication App { get; set; }
        public IConfigurationRoot Config { get; set; }
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        public SiteLookupService SiteLookupService { get; set; }
        public UserService UserService { get; set; }
        public ServiceFacade(CommandLineApplication app,
            IConfigurationRoot config,
            IHttpContextAccessor httpContextAccessor,
            IUserContextProvider userContextProvider,
            SiteLookupService siteLookupService,
            UserService userService)
        {
            App = app ?? throw new ArgumentNullException(nameof(app));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            HttpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            SiteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
    }
}
