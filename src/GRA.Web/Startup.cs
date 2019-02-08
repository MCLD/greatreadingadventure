using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using GRA.Abstract;
using GRA.Controllers.RouteConstraint;
using GRA.Domain.Model;
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
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace GRA.Web
{
    public class Startup
    {
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
            { ConfigurationKey.ContentPath, "content" }
        };

        //private static readonly CultureInfo CultureEnUs = new CultureInfo("en-US");
        //private static readonly CultureInfo CultureEsUs = new CultureInfo("es-US");

        //private static readonly IList<CultureInfo> SupportedCultures = new List<CultureInfo>
        //{
        //    CultureEnUs,
        //    CultureEsUs
        //};

        private readonly IConfiguration _config;
        private readonly bool _isDevelopment;
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

            _isDevelopment = env.IsDevelopment();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(_ =>
            {
                _.DefaultRequestCulture = new RequestCulture(Culture.DefaultCulture);
                _.SupportedCultures = Culture.SupportedCultures;
                _.SupportedUICultures = Culture.SupportedCultures;
            });

            // Add framework services.
            services.AddSession(_ => _.IdleTimeout = TimeSpan.FromMinutes(30));

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
                    break;
                default:
                    _logger.LogInformation("Using memory-based distributed cache");
                    services.AddDistributedMemoryCache();
                    break;
            }

            // add MVC
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            // Add custom view directory
            services.Configure<RazorViewEngineOptions>(options =>
                options.ViewLocationFormats.Insert(0, "/shared/views/{1}/{0}.cshtml")
            );

            // set cookie authentication options
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(_ =>
                {
                    _.LoginPath = "/SignIn";
                    _.LogoutPath = "/Home/Signout";
                    _.AccessDeniedPath = "/";
                });

            services.AddAuthorization(options =>
            {
                foreach (object permisisonName in Enum.GetValues(typeof(Domain.Model.Permission)))
                {
                    options.AddPolicy(permisisonName.ToString(),
                        _ => _.RequireClaim(ClaimType.Permission, permisisonName.ToString()));
                }

                options.AddPolicy(Policy.ActivateChallenges,
                    _ => _.RequireClaim(ClaimType.Permission,
                        nameof(Permission.ActivateAllChallenges),
                        nameof(Permission.ActivateSystemChallenges)));
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

            // configure ef errors to throw, log, or ignore as appropriate for the environment
            // see https://docs.microsoft.com/en-us/ef/core/querying/related-data#ignored-includes
            var throwEvents = new List<EventId>();
            var logEvents = new List<EventId>();
            var ignoreEvents = new List<EventId>();

            if (_isDevelopment)
            {
                logEvents.Add(RelationalEventId.QueryClientEvaluationWarning);
                throwEvents.Add(CoreEventId.IncludeIgnoredWarning);
            }
            else
            {
                if (string.IsNullOrEmpty(_config[ConfigurationKey.DatabaseWarningLogging]))
                {
                    ignoreEvents.Add(RelationalEventId.QueryClientEvaluationWarning);
                    ignoreEvents.Add(CoreEventId.IncludeIgnoredWarning);
                }
                else
                {
                    logEvents.Add(RelationalEventId.QueryClientEvaluationWarning);
                    logEvents.Add(CoreEventId.IncludeIgnoredWarning);
                }
            }

            string cs = _config.GetConnectionString(csName)
                ?? throw new Exception($"A {csName} connection string must be provided.");
            switch (_config[ConfigurationKey.ConnectionStringName])
            {
                case ConnectionStringNameSqlServer:
                    if (!string.IsNullOrEmpty(_config[ConfigurationKey.SqlServer2008]))
                    {
                        services.AddDbContextPool<Data.Context, Data.SqlServer.SqlServerContext>(
                            _ => _.UseSqlServer(cs, b => b.UseRowNumberForPaging())
                            .ConfigureWarnings(w => w
                                .Throw(throwEvents.ToArray())
                                .Log(logEvents.ToArray())
                                .Ignore(ignoreEvents.ToArray())));
                    }
                    else
                    {
                        services.AddDbContextPool<Data.Context, Data.SqlServer.SqlServerContext>(
                            _ => _.UseSqlServer(cs)
                            .ConfigureWarnings(w => w
                                .Throw(throwEvents.ToArray())
                                .Log(logEvents.ToArray())
                                .Ignore(ignoreEvents.ToArray())));
                    }
                    break;
                case ConnectionStringNameSQLite:
                    services.AddDbContextPool<Data.Context, Data.SQLite.SQLiteContext>(
                        _ => _.UseSqlite(cs)
                            .ConfigureWarnings(w => w
                                .Throw(throwEvents.ToArray())
                                .Log(logEvents.ToArray())
                                .Ignore(ignoreEvents.ToArray())));
                    break;
                default:
                    throw new Exception($"Unknown GraConnectionStringName: {csName}");
            }

            // store the data protection key in the database
            services.AddDataProtection().PersistKeysToDbContext<Data.Context>();

            // utilities
            services.AddScoped<ICodeGenerator, CodeGenerator>();
            services.AddScoped<ICodeSanitizer, CodeSanitizer>();
            services.AddScoped<IDateTimeProvider, CurrentDateTimeProvider>();
            services.AddScoped<IEntitySerializer, EntitySerializer>();
            services.AddScoped<IPasswordValidator, PasswordValidator>();
            services.AddScoped<IPathResolver, PathResolver>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IUserContextProvider, Controllers.UserContextProvider>();
            services.AddScoped<Security.Abstract.IPasswordHasher, Security.PasswordHasher>();
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
            services.AddScoped<CarouselService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<ChallengeService>();
            services.AddScoped<DailyLiteracyTipService>();
            services.AddScoped<DashboardContentService>();
            services.AddScoped<Domain.Report.ServiceFacade.Report>();
            services.AddScoped<DrawingService>();
            services.AddScoped<EmailManagementService>();
            services.AddScoped<EmailReminderService>();
            services.AddScoped<EmailService>();
            services.AddScoped<EventImportService>();
            services.AddScoped<EventService>();
            services.AddScoped<GroupTypeService>();
            services.AddScoped<LanguageService>();
            services.AddScoped<MailService>();
            services.AddScoped<NewsService>();
            services.AddScoped<PageService>();
            services.AddScoped<PerformerSchedulingService>();
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
            services.AddScoped<SpatialService>();
            services.AddScoped<SystemInformationService>();
            services.AddScoped<TemplateService>();
            services.AddScoped<TriggerService>();
            services.AddScoped<UserService>();
            services.AddScoped<VendorCodeService>();

            services.AddScoped<Domain.Report.ActivityByProgramReport>();
            services.AddScoped<Domain.Report.BadgeReport>();
            services.AddScoped<Domain.Report.BadgeTopScoresReport>();
            services.AddScoped<Domain.Report.CommunityExperiencesReport>();
            services.AddScoped<Domain.Report.CurrentStatusByProgramReport>();
            services.AddScoped<Domain.Report.CurrentStatusReport>();
            services.AddScoped<Domain.Report.GroupVendorCodeReport>();
            services.AddScoped<Domain.Report.ParticipantCountMinutesByProgram>();
            services.AddScoped<Domain.Report.ParticipantPrizeReport>();
            services.AddScoped<Domain.Report.ParticipantProgressReport>();
            services.AddScoped<Domain.Report.PrizeRedemptionReport>();
            services.AddScoped<Domain.Report.RegistrationsAchieversBySchoolReport>();
            services.AddScoped<Domain.Report.RegistrationsAchieversReport>();
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
            services.AddScoped<Domain.Repository.ICarouselItemRepository, Data.Repository.CarouselItemRepository>();
            services.AddScoped<Domain.Repository.ICarouselRepository, Data.Repository.CarouselRepository>();
            services.AddScoped<Domain.Repository.ICategoryRepository, Data.Repository.CategoryRepository>();
            services.AddScoped<Domain.Repository.IChallengeGroupRepository, Data.Repository.ChallengeGroupRepository>();
            services.AddScoped<Domain.Repository.IChallengeRepository, Data.Repository.ChallengeRepository>();
            services.AddScoped<Domain.Repository.IChallengeTaskRepository, Data.Repository.ChallengeTaskRepository>();
            services.AddScoped<Domain.Repository.IDailyLiteracyTipImageRepository, Data.Repository.DailyLiteracyTipImageRepository>();
            services.AddScoped<Domain.Repository.IDailyLiteracyTipRepository, Data.Repository.DailyLiteracyTipRepository>();
            services.AddScoped<Domain.Repository.IDashboardContentRepository, Data.Repository.DashboardContentRepository>();
            services.AddScoped<Domain.Repository.IDrawingCriterionRepository, Data.Repository.DrawingCriterionRepository>();
            services.AddScoped<Domain.Repository.IDrawingRepository, Data.Repository.DrawingRepository>();
            services.AddScoped<Domain.Repository.IEmailReminderRepository, Data.Repository.EmailReminderRepository>();
            services.AddScoped<Domain.Repository.IEmailSubscriptionAuditLogRepository, Data.Repository.EmailSubscriptionAuditLogRepository>();
            services.AddScoped<Domain.Repository.IEventRepository, Data.Repository.EventRepository>();
            services.AddScoped<Domain.Repository.IGroupInfoRepository, Data.Repository.GroupInfoRepository>();
            services.AddScoped<Domain.Repository.IGroupTypeRepository, Data.Repository.GroupTypeRepository>();
            services.AddScoped<Domain.Repository.ILanguageRepository, Data.Repository.LanguageRepository>();
            services.AddScoped<Domain.Repository.ILocationRepository, Data.Repository.LocationRepository>();
            services.AddScoped<Domain.Repository.IMailRepository, Data.Repository.MailRepository>();
            services.AddScoped<Domain.Repository.INewsCategoryRepository, Data.Repository.NewsCategoryRepository>();
            services.AddScoped<Domain.Repository.INewsPostRepository, Data.Repository.NewsPostRepository>();
            services.AddScoped<Domain.Repository.INotificationRepository, Data.Repository.NotificationRepository>();
            services.AddScoped<Domain.Repository.IPageRepository, Data.Repository.PageRepository>();
            services.AddScoped<Domain.Repository.IPointTranslationRepository, Data.Repository.PointTranslationRepository>();
            services.AddScoped<Domain.Repository.IPrizeWinnerRepository, Data.Repository.PrizeWinnerRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.IQuestionnaireRepository, Data.Repository.QuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IQuestionRepository, Data.Repository.QuestionRepository>();
            services.AddScoped<Domain.Repository.IRecoveryTokenRepository, Data.Repository.RecoveryTokenRepository>();
            services.AddScoped<Domain.Repository.IReportCriterionRepository, Data.Repository.ReportCriterionRepository>();
            services.AddScoped<Domain.Repository.IReportRequestRepository, Data.Repository.ReportRequestRepository>();
            services.AddScoped<Domain.Repository.IRequiredQuestionnaireRepository, Data.Repository.RequiredQuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IRoleRepository, Data.Repository.RoleRepository>();
            services.AddScoped<Domain.Repository.IPsAgeGroupRepository, Data.Repository.PsAgeGroupRepository>();
            services.AddScoped<Domain.Repository.IPsBlackoutDateRepository, Data.Repository.PsBlackoutDateRepository>();
            services.AddScoped<Domain.Repository.IPsBranchSelectionRepository, Data.Repository.PsBranchSelectionRepository>();
            services.AddScoped<Domain.Repository.IPsSettingsRepository, Data.Repository.PsSettingsRepository>();
            services.AddScoped<Domain.Repository.IPsKitImageRepository, Data.Repository.PsKitImageRepository>();
            services.AddScoped<Domain.Repository.IPsKitRepository, Data.Repository.PsKitRepository>();
            services.AddScoped<Domain.Repository.IPsPerformerImageRepository, Data.Repository.PsPerformerImageRepository>();
            services.AddScoped<Domain.Repository.IPsPerformerRepository, Data.Repository.PsPerformerRepository>();
            services.AddScoped<Domain.Repository.IPsPerformerScheduleRepository, Data.Repository.PsPerformerScheduleRepository>();
            services.AddScoped<Domain.Repository.IPsProgramImageRepository, Data.Repository.PsProgramImageRepository>();
            services.AddScoped<Domain.Repository.IPsProgramRepository, Data.Repository.PsProgramRepository>();
            services.AddScoped<Domain.Repository.ISchoolDistrictRepository, Data.Repository.SchoolDistrictRepository>();
            services.AddScoped<Domain.Repository.ISchoolRepository, Data.Repository.SchoolRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.ISiteSettingRepository, Data.Repository.SiteSettingRepository>();
            services.AddScoped<Domain.Repository.ISpatialDistanceRepository, Data.Repository.SpatialDistanceRepository>();
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
            IPathResolver pathResolver,
            Controllers.Base.ISitePathValidator sitePathValidator)
        {
            if (_isDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/Index/{0}");
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(Culture.DefaultCulture),
                SupportedCultures = Culture.SupportedCultures,
                SupportedUICultures = Culture.SupportedCultures
            });

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

            app.UseResponseCompression();

            // configure static files with 7 day cache
            app.UseStaticFiles(new StaticFileOptions
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
                catch (Exception)
                {
                    Console.WriteLine($"Unable to create directory '{contentPath}' in {Directory.GetCurrentDirectory()}");
                    throw;
                }
            }

            string pathString = pathResolver.ResolveContentPath();
            if (!pathString.StartsWith("/"))
            {
                pathString = "/" + pathString;
            }

            // https://github.com/aspnet/AspNetCore/issues/2442
            var extensionContentTypeProvider = new FileExtensionContentTypeProvider();
            extensionContentTypeProvider.Mappings[".webmanifest"] = "application/manifest+json";

            // configure /content with 7 day cache
            app.UseStaticFiles(new StaticFileOptions
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
                },
                ContentTypeProvider = extensionContentTypeProvider
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
