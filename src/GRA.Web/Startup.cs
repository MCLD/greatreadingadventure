using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using GRA.Abstract;
using GRA.Controllers.RouteConstraint;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using StackExchange.Redis;

namespace GRA.Web
{
    public class Startup
    {
        private const string DefaultCulture = "en-US";

        private const string ConfigurationSingleProgramValue = "Single";
        private const string ConfigurationMultipleProgramValue = "Multiple";

        private const string DefaultInitialProgramSetup = ConfigurationMultipleProgramValue;

        private const string ConnectionStringNameSqlServer = "SqlServer";
        private const string ConnectionStringNameSQLite = "SQLite";

        private readonly IDictionary<string, string> _defaultSettings = new Dictionary<string, string>
        {
            { ConfigurationKey.DefaultSiteName, "The Great Reading Adventure" },
            { ConfigurationKey.DefaultPageTitle, "Great Reading Adventure" },
            { ConfigurationKey.DefaultSitePath, "gra" },
            { ConfigurationKey.DefaultFooter, "This site is running the open source <a href=\"http://www.greatreadingadventure.com/\">Great Reading Adventure</a> software developed by the <a href=\"https://mcldaz.org/\">Maricopa County Library District</a> with support by the <a href=\"http://www.azlibrary.gov/\">Arizona State Library, Archives and Public Records</a>, a division of the Secretary of State, and with federal funds from the <a href=\"http://www.imls.gov/\">Institute of Museum and Library Services</a>." },
            { ConfigurationKey.InitialAuthorizationCode, "gra4adminmagic" },
            { ConfigurationKey.ContentPath, "content" },
            { ConfigurationKey.Culture, "en-US" }
        };

        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public Startup(IConfiguration config,
            IHostingEnvironment env,
            ILogger<Startup> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            foreach (string configKey in _defaultSettings.Keys)
            {
                if (string.IsNullOrEmpty(_config[configKey]))
                {
                    _config[configKey] = _defaultSettings[configKey];
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // set a default culture of en-US if none is specified
            string culture = _config[ConfigurationKey.Culture] ?? DefaultCulture;
            _logger.LogInformation("Configuring for culture: {0}", culture);
            services.Configure<RequestLocalizationOptions>(_ =>
            {
                _.DefaultRequestCulture = new RequestCulture(culture);
                _.SupportedCultures = new List<CultureInfo> { new CultureInfo(culture) };
                _.SupportedUICultures = new List<CultureInfo> { new CultureInfo(culture) };
            });

            // Add framework services.
            services.AddSession(_ =>
            {
                _.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.TryAddSingleton(_ => _config);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddResponseCompression(_ =>
            {
                _.Providers.Add<GzipCompressionProvider>();
                _.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                        "application/vnd.ms-fontobject",
                        "application/x-font-opentype",
                        "application/x-font-truetype",
                        "application/x-font-ttf",
                        "application/x-javascript",
                        "font/eot",
                        "font/opentype",
                        "font/otf",
                        "image/svg+xml",
                        "image/vnd.microsoft.icon",
                        "text/javascript"
                });
            });

            // configure distributed cache
            string protectionPath =
                !string.IsNullOrEmpty(_config[ConfigurationKey.DataProtectionPath])
                ? _config[ConfigurationKey.DataProtectionPath]
                : Path.Combine(Directory.GetCurrentDirectory(), "shared", "dataprotection");
            string discriminator = _config[ConfigurationKey.ApplicationDiscriminator]
                    ?? "gra";

            switch (_config[ConfigurationKey.DistributedCache]?.ToLower())
            {
                case "redis":
                    string redisConfig = _config[ConfigurationKey.RedisConfiguration]
                        ?? throw new Exception($"{ConfigurationKey.DistributedCache} has Redis selected but {ConfigurationKey.RedisConfiguration} is not set.");
                    string redisInstance = "gra." + discriminator;
                    if (!redisInstance.EndsWith("."))
                    {
                        redisInstance += ".";
                    }
                    services.AddDistributedRedisCache(_ =>
                    {
                        _.Configuration = redisConfig;
                        _.InstanceName = redisInstance;
                    });
                    var redis = ConnectionMultiplexer.Connect(redisConfig);
                    services.AddDataProtection(_ => _.ApplicationDiscriminator = discriminator)
                        .PersistKeysToRedis(redis, $"grainternal.{discriminator}.dpk")
                        .SetApplicationName(discriminator);
                    _logger.LogInformation("Using Redis distributed cache {0} discriminator {1}",
                        redisConfig,
                        redisInstance);
                    break;
                case "sqlserver":
                    string sessionCs = _config.GetConnectionString("SqlServerSessions")
                        ?? throw new Exception($"{ConfigurationKey.DistributedCache} has SQL Server selected but SqlServerSessions connection string is not set.");
                    string sessionTable = _config[ConfigurationKey.SqlSessionTable] ?? "Sessions";
                    _logger.LogInformation("Using SQL Server distributed cache in table {sessionTable}", sessionTable);
                    services.AddDistributedSqlServerCache(_ =>
                    {
                        _.ConnectionString = sessionCs;
                        _.SchemaName = _config[ConfigurationKey.SqlSessionSchemaName] ?? "dbo";
                        _.TableName = sessionTable;
                    });
                    services.AddDataProtection(_ => _.ApplicationDiscriminator = discriminator)
                        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(protectionPath, "keys")))
                        .SetApplicationName(discriminator);
                    break;
                default:
                    _logger.LogInformation("Using memory-based distributed cache");
                    services.AddDistributedMemoryCache();
                    services.AddDataProtection(_ => _.ApplicationDiscriminator = discriminator)
                        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(protectionPath, "keys")))
                        .SetApplicationName(discriminator);
                    break;
            }

            // add MVC
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            // Add custom view directory
            services.Configure<RazorViewEngineOptions>(options =>
                options.ViewLocationFormats.Insert(0, "/shared/views/{1}/{0}.cshtml")
            );

            // set cookie authentication options
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(_ => new CookieAuthenticationOptions
                {
                    LoginPath = new PathString("/SignIn/"),
                    AccessDeniedPath = new PathString("/"),
                });

            services.AddAuthorization(options =>
            {
                foreach (object permisisonName in Enum.GetValues(typeof(Domain.Model.Permission)))
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
            if (string.IsNullOrEmpty(_config[ConfigurationKey.ConnectionStringName]))
            {
                throw new Exception("GraConnectionStringName is not configured in appsettings.json - cannot continue");
            }

            string csName = _config[ConfigurationKey.ConnectionStringName]
                ?? throw new Exception($"{ConfigurationKey.ConnectionStringName} must be provided.");

            // mute ignored includes warnings
            // see https://docs.microsoft.com/en-us/ef/core/querying/related-data#ignored-includes
            var dbContextBuilder = new DbContextOptionsBuilder<Data.Context>();
            if (_logger.IsEnabled(LogLevel.Debug) || _logger.IsEnabled(LogLevel.Trace))
            {
                dbContextBuilder.ConfigureWarnings(w => w.Ignore());
            }

            string cs = _config.GetConnectionString(csName)
                ?? throw new Exception($"A {csName} connection string must be provided.");
            switch (_config[ConfigurationKey.ConnectionStringName])
            {
                case ConnectionStringNameSqlServer:
                    var sqlBuilder = new SqlServerDbContextOptionsBuilder(dbContextBuilder);
                    if (!string.IsNullOrEmpty(_config[ConfigurationKey.SqlServer2008]))
                    {
                        sqlBuilder.UseRowNumberForPaging();
                    }
                    services.AddDbContextPool<Data.Context,
                        Data.SqlServer.SqlServerContext>(
                        _ => _.UseSqlServer(cs, b => b = sqlBuilder));
                    break;
                case ConnectionStringNameSQLite:
                    var sqliteBuilder = new SqliteDbContextOptionsBuilder(dbContextBuilder);

                    services.AddDbContextPool<Data.Context, Data.SQLite.SQLiteContext>(
                        _ => _.UseSqlite(cs, b => b = sqliteBuilder));
                    break;
                default:
                    throw new Exception($"Unknown GraConnectionStringName: {csName}");
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
            services.AddScoped<AuthorizationCodeService>();
            services.AddScoped<AvatarService>();
            services.AddScoped<BadgeService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<ChallengeService>();
            services.AddScoped<DailyLiteracyTipService>();
            services.AddScoped<DashboardContentService>();
            services.AddScoped<DrawingService>();
            services.AddScoped<EmailReminderService>();
            services.AddScoped<EmailService>();
            services.AddScoped<EventImportService>();
            services.AddScoped<EventService>();
            services.AddScoped<GroupTypeService>();
            services.AddScoped<MailService>();
            services.AddScoped<PageService>();
            services.AddScoped<PointTranslationService>();
            services.AddScoped<PrizeWinnerService>();
            services.AddScoped<QuestionnaireService>();
            services.AddScoped<ReportService>();
            services.AddScoped<RoleService>();
            services.AddScoped<SampleDataService>();
            services.AddScoped<SchoolImportService>();
            services.AddScoped<SchoolService>();
            services.AddScoped<SiteLookupService>();
            services.AddScoped<SiteService>();
            services.AddScoped<SystemInformationService>();
            services.AddScoped<TemplateService>();
            services.AddScoped<TriggerService>();
            services.AddScoped<UserService>();
            services.AddScoped<VendorCodeService>();

            services.AddScoped<Domain.Report.ServiceFacade.Report>();
            services.AddScoped<Domain.Report.ActivityByProgramReport>();
            services.AddScoped<Domain.Report.BadgeReport>();
            services.AddScoped<Domain.Report.BadgeTopScoresReport>();
            services.AddScoped<Domain.Report.CommunityExperiencesReport>();
            services.AddScoped<Domain.Report.CurrentStatusByProgramReport>();
            services.AddScoped<Domain.Report.CurrentStatusReport>();
            services.AddScoped<Domain.Report.GroupVendorCodeReport>();
            services.AddScoped<Domain.Report.RegistrationsAchieversBySchoolReport>();
            services.AddScoped<Domain.Report.RegistrationsAchieversReport>();
            services.AddScoped<Domain.Report.ParticipantPrizeReport>();
            services.AddScoped<Domain.Report.ParticipantProgressReport>();
            services.AddScoped<Domain.Report.ParticipantCountMinutesByProgram>();
            services.AddScoped<Domain.Report.PrizeRedemptionReport>();
            services.AddScoped<Domain.Report.TopScoresReport>();
            services.AddScoped<Domain.Report.VendorCodeDonationsReport>();
            services.AddScoped<Domain.Report.VendorCodeReport>();

            // service resolution
            string initialProgramSetup = DefaultInitialProgramSetup;

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.InitialProgramSetup]))
            {
                initialProgramSetup = _config[ConfigurationKey.InitialProgramSetup];
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
            services.AddScoped<Domain.Repository.IAvatarBundleRepository, Data.Repository.AvatarBundleRepository>();
            services.AddScoped<Domain.Repository.IAvatarColorRepository, Data.Repository.AvatarColorRepository>();
            services.AddScoped<Domain.Repository.IAvatarElementRepository, Data.Repository.AvatarElementRepository>();
            services.AddScoped<Domain.Repository.IAvatarItemRepository, Data.Repository.AvatarItemRepository>();
            services.AddScoped<Domain.Repository.IAvatarLayerRepository, Data.Repository.AvatarLayerRepository>();
            services.AddScoped<Domain.Repository.IBadgeRepository, Data.Repository.BadgeRepository>();
            services.AddScoped<Domain.Repository.IBookRepository, Data.Repository.BookRepository>();
            services.AddScoped<Domain.Repository.IBranchRepository, Data.Repository.BranchRepository>();
            services.AddScoped<Domain.Repository.IBroadcastRepository, Data.Repository.BroadcastRepository>();
            services.AddScoped<Domain.Repository.ICategoryRepository, Data.Repository.CategoryRepository>();
            services.AddScoped<Domain.Repository.IChallengeRepository, Data.Repository.ChallengeRepository>();
            services.AddScoped<Domain.Repository.IChallengeGroupRepository, Data.Repository.ChallengeGroupRepository>();
            services.AddScoped<Domain.Repository.IChallengeTaskRepository, Data.Repository.ChallengeTaskRepository>();
            services.AddScoped<Domain.Repository.IDailyLiteracyTipImageRepository, Data.Repository.DailyLiteracyTipImageRepository>();
            services.AddScoped<Domain.Repository.IDailyLiteracyTipRepository, Data.Repository.DailyLiteracyTipRepository>();
            services.AddScoped<Domain.Repository.IDashboardContentRepository, Data.Repository.DashboardContentRepository>();
            services.AddScoped<Domain.Repository.IDrawingCriterionRepository, Data.Repository.DrawingCriterionRepository>();
            services.AddScoped<Domain.Repository.IDrawingRepository, Data.Repository.DrawingRepository>();
            services.AddScoped<Domain.Repository.IEmailReminderRepository, Data.Repository.EmailReminderRepository>();
            services.AddScoped<Domain.Repository.IEventRepository, Data.Repository.EventRepository>();
            services.AddScoped<Domain.Repository.IGroupInfoRepository, Data.Repository.GroupInfoRepository>();
            services.AddScoped<Domain.Repository.IGroupTypeRepository, Data.Repository.GroupTypeRepository>();
            services.AddScoped<Domain.Repository.ILocationRepository, Data.Repository.LocationRepository>();
            services.AddScoped<Domain.Repository.IMailRepository, Data.Repository.MailRepository>();
            services.AddScoped<Domain.Repository.INotificationRepository, Data.Repository.NotificationRepository>();
            services.AddScoped<Domain.Repository.IPageRepository, Data.Repository.PageRepository>();
            services.AddScoped<Domain.Repository.IPointTranslationRepository, Data.Repository.PointTranslationRepository>();
            services.AddScoped<Domain.Repository.IPrizeWinnerRepository, Data.Repository.PrizeWinnerRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.IQuestionRepository, Data.Repository.QuestionRepository>();
            services.AddScoped<Domain.Repository.IQuestionnaireRepository, Data.Repository.QuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IRecoveryTokenRepository, Data.Repository.RecoveryTokenRepository>();
            services.AddScoped<Domain.Repository.IRequiredQuestionnaireRepository, Data.Repository.RequiredQuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IReportCriterionRepository, Data.Repository.ReportCriterionRepository>();
            services.AddScoped<Domain.Repository.IReportRequestRepository, Data.Repository.ReportRequestRepository>();
            services.AddScoped<Domain.Repository.IRoleRepository, Data.Repository.RoleRepository>();
            services.AddScoped<Domain.Repository.ISchoolDistrictRepository, Data.Repository.SchoolDistrictRepository>();
            services.AddScoped<Domain.Repository.ISchoolRepository, Data.Repository.SchoolRepository>();
            services.AddScoped<Domain.Repository.ISchoolTypeRepository, Data.Repository.SchoolTypeRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.ISiteSettingRepository, Data.Repository.SiteSettingRepository>();
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
            IPathResolver pathResolver,
            Controllers.Base.ISitePathValidator sitePathValidator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/Index/{0}");
            }

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.ReverseProxyAddress]))
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All,
                    RequireHeaderSymmetry = false,
                    ForwardLimit = null,
                    KnownProxies = {
                        System.Net.IPAddress.Parse(_config[ConfigurationKey.ReverseProxyAddress])
                    }
                });
            }

            // insert remote address and trace identifier into the log context for each request
            app.Use(async (context, next) =>
            {
                using (LogContext.PushProperty("Identifier", context.TraceIdentifier))
                using (LogContext.PushProperty("RemoteAddress", context.Connection.RemoteIpAddress))
                {
                    await next.Invoke();
                }
            });

            app.UseRequestLocalization();

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

            app.UseAuthentication();

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
                        sitePath = new SiteRouteConstraint(sitePathValidator)
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
                        sitePath = new SiteRouteConstraint(sitePathValidator)
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
                    var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
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
