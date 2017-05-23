using System;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace GRA.CommandLine
{
    public class ServiceFacade
    {
        public CommandLineApplication App { get; private set; }
        public IConfigurationRoot Config { get; private set; }
        public IHttpContextAccessor HttpContextAccessor { get; private set; }
        public AuthenticationService AuthenticationService { get; private set; }
        public SiteLookupService SiteLookupService { get; private set; }
        public UserService UserService { get; private set; }
        public ServiceFacade(CommandLineApplication app,
            IConfigurationRoot config,
            IHttpContextAccessor httpContextAccessor,
            IUserContextProvider userContextProvider,
            AuthenticationService authenticationService,
            SiteLookupService siteLookupService,
            UserService userService)
        {
            App = app ?? throw new ArgumentNullException(nameof(app));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            HttpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            AuthenticationService = authenticationService 
                ?? throw new ArgumentNullException(nameof(authenticationService));
            SiteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
    }
}
