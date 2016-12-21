using AutoMapper;
using GRA.Controllers.RouteConstraint;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace GRA.Web
{
    public class Startup
    {
        private IDictionary<string, string> _defaultSettings = new Dictionary<string, string>
        {
            { ConfigurationKey.DefaultSiteName, "The Great Reading Adventure" },
            { ConfigurationKey.DefaultPageTitle, "Great Reading Adventure" },
            { ConfigurationKey.DefaultSitePath, "gra" },
            { ConfigurationKey.DefaultFooter, "This site is running the open source <a href=\"http://www.greatreadingadventure.com/\">Great Reading Adventure</a> software developed by the <a href=\"https://mcldaz.org/\">Maricopa County Library District</a> with support by the <a href=\"http://www.azlibrary.gov/\">Arizona State Library, Archives and Public Records</a>, a division of the Secretary of State, and with federal funds from the <a href=\"http://www.imls.gov/\">Institute of Museum and Library Services</a>." },
            { ConfigurationKey.InitialAuthorizationCode, "gra4adminmagic" },
            //{ ConfigurationKey.ContentDirectory, @"c:\inetpub\content\" },
            { ConfigurationKey.ContentPath, "content" }
        };

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }
            builder.AddEnvironmentVariables();

            Configuration = builder.Build();

            foreach (var configKey in _defaultSettings.Keys)
            {
                if (string.IsNullOrEmpty(Configuration[configKey]))
                {
                    Configuration[configKey] = _defaultSettings[configKey];
                }
            }

            if (env.IsDevelopment())
            {
                if (string.IsNullOrEmpty(Configuration[ConfigurationKey.DefaultCSSqlServer]))
                {
                    Configuration[ConfigurationKey.DefaultCSSqlServer] 
                        = DefaultConnectionString.SqlServer;
                }
                if (string.IsNullOrEmpty(Configuration[ConfigurationKey.DefaultCSSQLite]))
                {
                    Configuration[ConfigurationKey.DefaultCSSQLite] 
                        = DefaultConnectionString.SQLite;
                }
            }

            string contentDirectory = Configuration[ConfigurationKey.ContentDirectory];

            if (!string.IsNullOrEmpty(contentDirectory) && !Directory.Exists(contentDirectory))
            {
                try
                {
                    Directory.CreateDirectory(contentDirectory);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to create directory '{contentDirectory}' in {Directory.GetCurrentDirectory()}");
                    throw (ex);
                }
            }
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddSession(_ =>
            {
                _.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.TryAddSingleton(_ => Configuration);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                foreach (var permisisonName in Enum.GetValues(typeof(Domain.Model.Permission)))
                {
                    options.AddPolicy(permisisonName.ToString(),
                        _ => _.RequireClaim(ClaimType.Permission, permisisonName.ToString()));
                }
            });

            // path validator
            services.AddScoped<Controllers.Base.ISitePathValidator, Controllers.Validator.SitePathValidator>();

            // service facades
            services.AddScoped<Controllers.ServiceFacade.Controller, Controllers.ServiceFacade.Controller>();
            services.AddScoped<Data.ServiceFacade.Repository, Data.ServiceFacade.Repository>();

            // database
            services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();
            //services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();

            // utilities
            services.AddScoped<Domain.Service.Abstract.IUserContextProvider, Controllers.UserContextProvider>();
            services.AddScoped<Security.Abstract.IPasswordHasher, Security.PasswordHasher>();
            services.AddScoped<Abstract.ITokenGenerator, TokenGenerator>();
            services.AddScoped<Abstract.IEntitySerializer, EntitySerializer>();

            // filters
            services.AddScoped<Controllers.Filter.NotificationFilter>();
            services.AddScoped<Controllers.Filter.SiteFilter>();

            // services
            services.AddScoped<ActivityService>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<BadgeService>();
            services.AddScoped<ChallengeService>();
            services.AddScoped<EmailReminderService>();
            services.AddScoped<EmailService>();
            services.AddScoped<InitialSetupService>();
            services.AddScoped<MailService>();
            services.AddScoped<PageService>();
            services.AddScoped<SampleDataService>();
            services.AddScoped<SiteLookupService>();
            services.AddScoped<SiteService>();
            services.AddScoped<StaticAvatarService>();
            services.AddScoped<UserService>();

            // repositories
            services.AddScoped<Domain.Repository.IAuthorizationCodeRepository, Data.Repository.AuthorizationCodeRepository>();
            services.AddScoped<Domain.Repository.IBadgeRepository, Data.Repository.BadgeRepository>();
            services.AddScoped<Domain.Repository.IBookRepository, Data.Repository.BookRepository>();
            services.AddScoped<Domain.Repository.IBranchRepository, Data.Repository.BranchRepository>();
            services.AddScoped<Domain.Repository.IChallengeRepository, Data.Repository.ChallengeRepository>();
            services.AddScoped<Domain.Repository.IChallengeTaskRepository, Data.Repository.ChallengeTaskRepository>();
            services.AddScoped<Domain.Repository.IEmailReminderRepository, Data.Repository.EmailReminderRepository>();
            services.AddScoped<Domain.Repository.IMailRepository, Data.Repository.MailRepository>();
            services.AddScoped<Domain.Repository.INotificationRepository, Data.Repository.NotificationRepository>();
            services.AddScoped<Domain.Repository.IPageRepository, Data.Repository.PageRepository>();
            services.AddScoped<Domain.Repository.IPointTranslationRepository, Data.Repository.PointTranslationRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.IRecoveryTokenRepository, Data.Repository.RecoveryTokenRepository>();
            services.AddScoped<Domain.Repository.IRoleRepository, Data.Repository.RoleRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.IStaticAvatarRepository, Data.Repository.StaticAvatarRepository>();
            services.AddScoped<Domain.Repository.ISystemRepository, Data.Repository.SystemRepository>();
            services.AddScoped<Domain.Repository.IUserLogRepository, Data.Repository.UserLogRepository>();
            services.AddScoped<Domain.Repository.IUserRepository, Data.Repository.UserRepository>();

            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/gra-{Date}.txt");
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.ApplicationServices.GetService<Data.Context>().Migrate();

            // configure static files with 7 day cache
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = _ =>
                {
                    var headers = _.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(7)
                    };
                }
            });

            // configure /content with 7 day cache
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "content")),
                RequestPath = new PathString("/content"),
                OnPrepareResponse = _ =>
                {
                    var headers = _.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(7)
                    };
                }
            });

            app.UseSession();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = Controllers.Authentication.SchemeGRACookie,
                LoginPath = new PathString("/SignIn/"),
                AccessDeniedPath = new PathString("/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            // sitePath is also referenced in GRA.Controllers.Filter.SiteFilter
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: null,
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: null,
                    template: "{sitePath}/Info/{stub}",
                    defaults: new { controller = "Info", action = "Index" },
                    constraints: new
                    {
                        sitePath = new SiteRouteConstraint(app.ApplicationServices.GetRequiredService<Controllers.Base.ISitePathValidator>())
                    });
                routes.MapRoute(
                    name: null,
                    template: "Info/{stub}",
                    defaults: new { controller = "Info", action = "Index" });

                routes.MapRoute(
                    name: null,
                    template: "{sitePath}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" },
                    constraints: new
                    {
                        sitePath = new SiteRouteConstraint(app.ApplicationServices.GetRequiredService<Controllers.Base.ISitePathValidator>())
                    });
                routes.MapRoute(
                    name: null,
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
