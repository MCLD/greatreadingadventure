using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using GRA.Abstract;
using GRA.Controllers.RouteConstraint;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GRA.Web
{
    public class Startup
    {
        private const string ConfigurationSingleProgramValue = "Single";
        private const string ConfigurationMultipleProgramValue = "Multiple";

        private const string DefaultInitialProgramSetup = ConfigurationSingleProgramValue;

        private const string ConnectionStringNameSqlServer = "SqlServer";
        private const string ConnectionStringNameSQLite = "SQLite";

        private readonly IDictionary<string, string> _defaultSettings = new Dictionary<string, string>
        {
            { ConfigurationKey.DefaultSiteName, "The Great Reading Adventure" },
            { ConfigurationKey.DefaultPageTitle, "Great Reading Adventure" },
            { ConfigurationKey.DefaultSitePath, "gra" },
            { ConfigurationKey.DefaultFooter, "This site is running the open source <a href=\"http://www.greatreadingadventure.com/\">Great Reading Adventure</a> software developed by the <a href=\"https://mcldaz.org/\">Maricopa County Library District</a> with support by the <a href=\"http://www.azlibrary.gov/\">Arizona State Library, Archives and Public Records</a>, a division of the Secretary of State, and with federal funds from the <a href=\"http://www.imls.gov/\">Institute of Museum and Library Services</a>." },
            { ConfigurationKey.InitialAuthorizationCode, "gra4adminmagic" },
            { ConfigurationKey.ContentPath, "content" }
        };

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true)
                .AddJsonFile("shared/appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"shared/appsettings.{env.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
            string instance =
                !string.IsNullOrEmpty(Configuration[ConfigurationKey.InstanceName])
                ? Configuration[ConfigurationKey.InstanceName]
                : "gra";

            if (!string.IsNullOrEmpty(Configuration[ConfigurationKey.RollingLogPath]))
            {
                string path = Configuration[ConfigurationKey.RollingLogPath];

                if (!path.EndsWith("/"))
                {
                    path = path + "/";
                }

                string rollingFilename = path + instance + "-{Date}.txt";

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
                    .WriteTo.RollingFile(rollingFilename)
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
                    .CreateLogger();
            }

            Log.Logger.Warning(string.Format("GRA{0} v{1} starting up in '{2}'",
                instance.ToLower() != "gra" ? " " + instance : string.Empty,
                Assembly.GetEntryAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion,
                env.WebRootPath));

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
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string protectionPath =
                !string.IsNullOrEmpty(Configuration[ConfigurationKey.DataProtectionPath])
                ? Configuration[ConfigurationKey.DataProtectionPath]
                : Path.Combine(Directory.GetCurrentDirectory(), "shared", "dataprotection");
            string discriminator = Configuration[ConfigurationKey.ApplicationDescriminator]
                    ?? "gra";
            services.AddDataProtection(_ => _.ApplicationDiscriminator = discriminator)
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(protectionPath, "keys")));

            // Add framework services.
            services.AddSession(_ =>
            {
                _.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.TryAddSingleton(_ => Configuration);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<Microsoft.AspNetCore.Mvc.ViewFeatures.IHtmlGenerator,
                HtmlGeneratorHack>();
            services.AddResponseCompression();
            services.AddMemoryCache();

            // check for a connection string for storing sessions in a database table
            string sessionCs = Configuration.GetConnectionString("SqlServerSessions");
            string sessionSchema = Configuration[ConfigurationKey.SqlSessionSchemaName] ?? "dbo";
            string sessionTable = Configuration[ConfigurationKey.SqlSessionTable] ?? "Sessions";
            if (!string.IsNullOrEmpty(sessionCs))
            {
                services.AddDistributedSqlServerCache(_ =>
                {
                    _.ConnectionString = sessionCs;
                    _.SchemaName = sessionSchema;
                    _.TableName = sessionTable;
                });
            }
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                foreach (var permisisonName in Enum.GetValues(typeof(Domain.Model.Permission)))
                {
                    options.AddPolicy(permisisonName.ToString(),
                        _ => _.RequireClaim(ClaimType.Permission, permisisonName.ToString()));
                }

                options.AddPolicy(Domain.Model.Policy.ActivateChallenges,
                    _ => _.RequireClaim(ClaimType.Permission,
                        Domain.Model.Permission.ActivateAllChallenges.ToString(),
                        Domain.Model.Permission.ActivateSystemChallenges.ToString()));
            });

            // path validator
            services.AddScoped<Controllers.Base.ISitePathValidator,
                Controllers.Validator.SitePathValidator>();

            // service facades
            services.AddScoped<Controllers.ServiceFacade.Controller,
                Controllers.ServiceFacade.Controller>();
            services.AddScoped<Data.ServiceFacade.Repository, Data.ServiceFacade.Repository>();

            // database
            if (string.IsNullOrEmpty(Configuration[ConfigurationKey.ConnectionStringName]))
            {
                throw new Exception("GraConnectionStringName is not configured in appsettings.json - cannot continue");
            }

            switch (Configuration[ConfigurationKey.ConnectionStringName])
            {
                case ConnectionStringNameSqlServer:
                    services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();
                    break;
                case ConnectionStringNameSQLite:
                    services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();
                    break;
                default:
                    throw new Exception($"Unknown GraConnectionStringName: {Configuration[ConfigurationKey.ConnectionStringName]}");
            }

            // utilities
            services.AddScoped<IDateTimeProvider, CurrentDateTimeProvider>();
            services.AddScoped<IUserContextProvider, Controllers.UserContextProvider>();
            services.AddScoped<Security.Abstract.IPasswordHasher, Security.PasswordHasher>();
            services.AddScoped<IPasswordValidator, PasswordValidator>();
            services.AddScoped<IPathResolver, PathResolver>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IEntitySerializer, EntitySerializer>();
            services.AddScoped<ICodeGenerator, CodeGenerator>();
            services.AddScoped<ICodeSanitizer, CodeSanitizer>();
            services.AddScoped<WebSocketHandler>();

            // filters
            services.AddScoped<Controllers.Filter.MissionControlFilter>();
            services.AddScoped<Controllers.Filter.NotificationFilter>();
            services.AddScoped<Controllers.Filter.SiteFilter>();
            services.AddScoped<Controllers.Filter.UserFilter>();

            // services
            services.AddScoped<ActivityService>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<BadgeService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<ChallengeService>();
            services.AddScoped<DrawingService>();
            services.AddScoped<DynamicAvatarService>();
            services.AddScoped<EmailReminderService>();
            services.AddScoped<EmailService>();
            services.AddScoped<EventImportService>();
            services.AddScoped<EventService>();
            services.AddScoped<MailService>();
            services.AddScoped<PageService>();
            services.AddScoped<PrizeWinnerService>();
            services.AddScoped<QuestionnaireService>();
            services.AddScoped<ReportService>();
            services.AddScoped<SampleDataService>();
            services.AddScoped<SchoolImportService>();
            services.AddScoped<SchoolService>();
            services.AddScoped<SiteLookupService>();
            services.AddScoped<SiteService>();
            services.AddScoped<StaticAvatarService>();
            services.AddScoped<SystemInformationService>();
            services.AddScoped<TriggerService>();
            services.AddScoped<UserService>();
            services.AddScoped<VendorCodeService>();

            services.AddScoped<Domain.Report.ServiceFacade.Report>();
            services.AddScoped<Domain.Report.ActivityByProgramReport>();
            services.AddScoped<Domain.Report.BadgeReport>();
            services.AddScoped<Domain.Report.BadgeTopScoresReport>();
            services.AddScoped<Domain.Report.CurrentStatusByProgramReport>();
            services.AddScoped<Domain.Report.CurrentStatusReport>();
            services.AddScoped<Domain.Report.RegistrationsAchieversBySchoolReport>();
            services.AddScoped<Domain.Report.RegistrationsAchieversReport>();
            services.AddScoped<Domain.Report.ParticipantPrizeReport>();
            services.AddScoped<Domain.Report.ParticipantProgressReport>();
            services.AddScoped<Domain.Report.PrizeRedemptionReport>();
            services.AddScoped<Domain.Report.TopScoresReport>();
            services.AddScoped<Domain.Report.VendorCodeReport>();

            // service resolution
            string initialProgramSetup = DefaultInitialProgramSetup;

            if (!string.IsNullOrEmpty(Configuration[ConfigurationKey.InitialProgramSetup]))
            {
                initialProgramSetup = Configuration[ConfigurationKey.InitialProgramSetup];
            }

            switch (initialProgramSetup)
            {
                case ConfigurationMultipleProgramValue:
                    services.AddScoped<IInitialSetupService, SetupMultipleProgramService>();
                    break;
                case ConfigurationSingleProgramValue:
                    services.AddScoped<IInitialSetupService, SetupSingleProgramService>();
                    break;
                default:
                    throw new Exception($"Unable to perform initial setup - unrecognized GraDefaultProgramSetup: {initialProgramSetup}");
            }

            // repositories
            services.AddScoped<Domain.Repository.IAnswerRepository, Data.Repository.AnswerRepository>();
            services.AddScoped<Domain.Repository.IAuthorizationCodeRepository, Data.Repository.AuthorizationCodeRepository>();
            services.AddScoped<Domain.Repository.IBadgeRepository, Data.Repository.BadgeRepository>();
            services.AddScoped<Domain.Repository.IBookRepository, Data.Repository.BookRepository>();
            services.AddScoped<Domain.Repository.IBranchRepository, Data.Repository.BranchRepository>();
            services.AddScoped<Domain.Repository.IBroadcastRepository, Data.Repository.BroadcastRepository>();
            services.AddScoped<Domain.Repository.IDrawingCriterionRepository, Data.Repository.DrawingCriterionRepository>();
            services.AddScoped<Domain.Repository.IDrawingRepository, Data.Repository.DrawingRepository>();
            services.AddScoped<Domain.Repository.IDynamicAvatarBundleRepository, Data.Repository.DynamicAvatarBundleRepository>();
            services.AddScoped<Domain.Repository.IDynamicAvatarColorRepository, Data.Repository.DynamicAvatarColorRepository>();
            services.AddScoped<Domain.Repository.IDynamicAvatarElementRepository, Data.Repository.DynamicAvatarElementRepository>();
            services.AddScoped<Domain.Repository.IDynamicAvatarItemRepository, Data.Repository.DynamicAvatarItemRepository>();
            services.AddScoped<Domain.Repository.IDynamicAvatarLayerRepository, Data.Repository.DynamicAvatarLayerRepository>();
            services.AddScoped<Domain.Repository.ICategoryRepository, Data.Repository.CategoryRepository>();
            services.AddScoped<Domain.Repository.IChallengeRepository, Data.Repository.ChallengeRepository>();
            services.AddScoped<Domain.Repository.IChallengeTaskRepository, Data.Repository.ChallengeTaskRepository>();
            services.AddScoped<Domain.Repository.IEmailReminderRepository, Data.Repository.EmailReminderRepository>();
            services.AddScoped<Domain.Repository.IEnteredSchoolRepository, Data.Repository.EnteredSchoolRepository>();
            services.AddScoped<Domain.Repository.IEventRepository, Data.Repository.EventRepository>();
            services.AddScoped<Domain.Repository.ILocationRepository, Data.Repository.LocationRepository>();
            services.AddScoped<Domain.Repository.IMailRepository, Data.Repository.MailRepository>();
            services.AddScoped<Domain.Repository.INotificationRepository, Data.Repository.NotificationRepository>();
            services.AddScoped<Domain.Repository.IPageRepository, Data.Repository.PageRepository>();
            services.AddScoped<Domain.Repository.IPointTranslationRepository, Data.Repository.PointTranslationRepository>();
            services.AddScoped<Domain.Repository.IPrizeWinnerRepository, Data.Repository.PrizeWinnerRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.IReportCriterionRepository, Data.Repository.ReportCriterionRepository>();
            services.AddScoped<Domain.Repository.IReportRequestRepository, Data.Repository.ReportRequestRepository>();
            services.AddScoped<Domain.Repository.IRequiredQuestionnaireRepository, Data.Repository.RequiredQuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IQuestionRepository, Data.Repository.QuestionRepository>();
            services.AddScoped<Domain.Repository.IQuestionnaireRepository, Data.Repository.QuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IRecoveryTokenRepository, Data.Repository.RecoveryTokenRepository>();
            services.AddScoped<Domain.Repository.IRoleRepository, Data.Repository.RoleRepository>();
            services.AddScoped<Domain.Repository.ISchoolDistrictRepository, Data.Repository.SchoolDistrictRepository>();
            services.AddScoped<Domain.Repository.ISchoolRepository, Data.Repository.SchoolRepository>();
            services.AddScoped<Domain.Repository.ISchoolTypeRepository, Data.Repository.SchoolTypeRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.IStaticAvatarRepository, Data.Repository.StaticAvatarRepository>();
            services.AddScoped<Domain.Repository.ISystemInformationRepository, Data.Repository.SystemInformationRepository>();
            services.AddScoped<Domain.Repository.ISystemRepository, Data.Repository.SystemRepository>();
            services.AddScoped<Domain.Repository.ITriggerRepository, Data.Repository.TriggerRepository>();
            services.AddScoped<Domain.Repository.IUserLogRepository, Data.Repository.UserLogRepository>();
            services.AddScoped<Domain.Repository.IUserRepository, Data.Repository.UserRepository>();
            services.AddScoped<Domain.Repository.IVendorCodeRepository, Data.Repository.VendorCodeRepository>();
            services.AddScoped<Domain.Repository.IVendorCodeTypeRepository, Data.Repository.VendorCodeTypeRepository>();

            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IPathResolver pathResolver)
        {
            loggerFactory.AddSerilog();

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

            app.UseResponseCompression();

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

            string contentPath = pathResolver.ResolveContentFilePath();

            if (!Directory.Exists(contentPath))
            {
                try
                {
                    Directory.CreateDirectory(contentPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to create directory '{contentPath}' in {Directory.GetCurrentDirectory()}");
                    throw (ex);
                }
            }

            string pathString = pathResolver.ResolveContentPath();
            if (!pathString.StartsWith("/"))
            {
                pathString = "/" + pathString;
            }

            // configure /content with 7 day cache
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(contentPath),
                RequestPath = new PathString(pathString),
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

            // set cookie authentication options
            var cookieAuthOptions = new CookieAuthenticationOptions
            {
                AuthenticationScheme = Controllers.Authentication.SchemeGRACookie,
                LoginPath = new PathString("/SignIn/"),
                AccessDeniedPath = new PathString("/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            };

            // if there's a data protection path, set it up - for clustered/multi-server configs
            if (!string.IsNullOrEmpty(Configuration[ConfigurationKey.DataProtectionPath]))
            {
                string protectionPath = Configuration[ConfigurationKey.DataProtectionPath];
                cookieAuthOptions.DataProtectionProvider = DataProtectionProvider.Create(
                    new DirectoryInfo(Path.Combine(protectionPath, "cookies")));
            }

            app.UseCookieAuthentication(cookieAuthOptions);

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

            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30)
            });

            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var handler = app.ApplicationServices.GetService<WebSocketHandler>();
                    await handler.Handle(context);
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
