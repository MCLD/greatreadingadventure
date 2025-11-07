using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GRA.Abstract;
using GRA.Controllers.RouteConstraint;
using GRA.Data.Config;
using GRA.Domain.Model;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Context;

namespace GRA.Web
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
        "CA1506: Avoid excessive class coupling",
        Justification = "Dependency injected methods lead to large amounts of coupling")]
    public class Startup
    {
        private const string ConfigurationMultipleProgramValue = "Multiple";
        private const string ConfigurationSingleProgramValue = "Single";
        private const string ConnectionStringNameSQLite = "SQLite";
        private const string ConnectionStringNameSqlServer = "SqlServer";
        private const string DefaultInitialProgramSetup = ConfigurationMultipleProgramValue;
        private const string ErrorControllerPath = "/Error";
        private readonly IConfiguration _config;
        private readonly bool _isDevelopment;

        public Startup(IConfiguration config,
            IWebHostEnvironment env)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            var defaults = new Dictionary<string, string>
            {
                { ConfigurationKey.DefaultSiteName, "The Great Reading Adventure" },
                { ConfigurationKey.DefaultPageTitle, "Great Reading Adventure" },
                { ConfigurationKey.DefaultSitePath, "gra" },
                { ConfigurationKey.DefaultFooter, "This site is running the open source <a href=\"https://www.greatreadingadventure.com/\">Great Reading Adventure</a> software developed by the <a href=\"https://mcldaz.org/\">Maricopa County Library District</a> with support by the <a href=\"https://azlibrary.gov/\">Arizona State Library, Archives and Public Records</a>, a division of the Secretary of State, and with federal funds from the <a href=\"https://imls.gov/\">Institute of Museum and Library Services</a>." },
                { ConfigurationKey.InitialAuthorizationCode, "gra4adminmagic" },
                { ConfigurationKey.ContentPath, "content" }
            };

            foreach (string configKey in defaults.Keys)
            {
                if (string.IsNullOrEmpty(_config[configKey]))
                {
                    _config[configKey] = defaults[configKey];
                }
            }

            _isDevelopment = env.IsDevelopment();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IPathResolver pathResolver)
        {
            ArgumentNullException.ThrowIfNull(app);
            ArgumentNullException.ThrowIfNull(pathResolver);

            if (_isDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(ErrorControllerPath);
            }

            app.UseStatusCodePagesWithReExecute(string.Join("/",
                "",
                Controllers.ErrorController.Name,
                nameof(Controllers.ErrorController.Index),
                "{0}"));

            // override proxy IP address if one is present
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

            // insert remote address, referer, and user agent into the logging enrichment
            app.Use(async (context, next) =>
            {
                using (LogContext.PushProperty(LoggingEnrichment.RemoteAddress,
                    context.Connection.RemoteIpAddress))
                using (LogContext.PushProperty(HeaderNames.Referer,
                    context.Request.GetTypedHeaders().Referer))
                using (LogContext.PushProperty(HeaderNames.UserAgent,
                context.Request.Headers[HeaderNames.UserAgent].ToString()))
                {
                    await next.Invoke();
                }
            });

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(Culture.DefaultCulture),
                SupportedCultures = Culture.SupportedCultures,
                SupportedUICultures = Culture.SupportedCultures
            };
            requestLocalizationOptions.RequestCultureProviders.Insert(0,
                new RouteDataRequestCultureProvider { Options = requestLocalizationOptions });

            requestLocalizationOptions
                .RequestCultureProviders
                .Remove(requestLocalizationOptions
                    .RequestCultureProviders
                    .Single(_ => _.GetType() == typeof(QueryStringRequestCultureProvider)));

            app.UseRequestLocalization(requestLocalizationOptions);

            if (!_isDevelopment)
            {
                app.UseResponseCompression();
            }

            app.UseWebOptimizer();

            // configure static files with 7 day cache
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = _ =>
                {
                    var headers = _.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
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
                    throw new GraException($"Unable to create directory '{contentPath}'", ex);
                }
            }

            string pathString = pathResolver.ResolveContentPath();
            if (!pathString.StartsWith("/", StringComparison.OrdinalIgnoreCase))
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
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        MaxAge = TimeSpan.FromDays(7)
                    };
                },
                ContentTypeProvider = extensionContentTypeProvider
            });

            if (!string.IsNullOrEmpty(_config[ConfigurationKey.EnableRequestLogging]))
            {
                app.UseSerilogRequestLogging();
            }

            app.UseSession();

            app.Use(async (context, next) =>
            {
                using (LogContext.PushProperty(LoggingEnrichment.UserId,
                    context.User.Claims.FirstOrDefault(_ => _.Type == ClaimType.UserId)?.Value))
                {
                    await next();
                }
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // sitePath is also referenced in GRA.Controllers.Filter.SiteFilterAttribute
            app.UseEndpoints(_ =>
            {
                _.MapControllerRoute(
                    name: null,
                    pattern: "{culture:cultureConstraint}/Info/{id}",
                    defaults: new { controller = "Info", action = "Index" });
                _.MapControllerRoute(
                    name: null,
                    pattern: "Info/{id}",
                    defaults: new { controller = "Info", action = "Index" });
                _.MapControllerRoute(
                    name: null,
                    pattern: "{culture:cultureConstraint}/{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
                _.MapControllerRoute(
                    name: null,
                    pattern: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
                _.MapControllerRoute(
                    name: null,
                    pattern: "{culture:cultureConstraint}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
                _.MapControllerRoute(
                    name: null,
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
                _.MapHealthChecks("/healthcheck");
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(_ =>
            {
                _.DefaultRequestCulture = new RequestCulture(Culture.DefaultCulture);
                _.SupportedCultures = Culture.SupportedCultures;
                _.SupportedUICultures = Culture.SupportedCultures;
                _.RequestCultureProviders.Insert(0,
                    new RouteDataRequestCultureProvider { Options = _ });
                _.RequestCultureProviders
                    .Remove(_.RequestCultureProviders
                        .Single(p => p.GetType() == typeof(QueryStringRequestCultureProvider)));
            });

            string discriminator = _config[ConfigurationKey.ApplicationDiscriminator]
                ?? "gra";

            // Add framework services.
            services.AddSession(_ =>
            {
                _.IdleTimeout = TimeSpan.FromMinutes(30);
                _.Cookie.IsEssential = true;
                _.Cookie.HttpOnly = true;
                _.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                _.Cookie.Name = $"{discriminator}-session";
            });

            services.TryAddSingleton(_ => _config);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (!_isDevelopment)
            {
                services.AddResponseCompression(_ =>
                {
                    _.Providers.Add<GzipCompressionProvider>();
                    _.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
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
                    ]);
                });
            }

            switch (_config[ConfigurationKey.DistributedCache]?.ToLower(Culture.DefaultCulture))
            {
                case "redis":
                    string redisConfig = _config[ConfigurationKey.RedisConfiguration]
                        ?? throw new GraFatalException($"{ConfigurationKey.DistributedCache} has Redis selected but {ConfigurationKey.RedisConfiguration} is not set.");
                    string redisInstance = "gra." + discriminator;
                    if (!redisInstance.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                    {
                        redisInstance += ".";
                    }
                    services.AddStackExchangeRedisCache(_ =>
                    {
                        _.Configuration = redisConfig;
                        _.InstanceName = redisInstance;
                    });
                    _config[ConfigurationKey.RuntimeCacheRedisConfiguration] = redisConfig;
                    _config[ConfigurationKey.RuntimeCacheRedisInstance] = redisInstance;
                    break;

                case "sqlserver":
                    string sessionCs = _config.GetConnectionString("SqlServerSessions")
                        ?? throw new GraFatalException($"{ConfigurationKey.DistributedCache} has SQL Server selected but SqlServerSessions connection string is not set.");
                    string sessionTable = _config[ConfigurationKey.SqlSessionTable] ?? "Sessions";
                    _config[ConfigurationKey.RuntimeCacheSqlConfiguration] = sessionTable;
                    services.AddDistributedSqlServerCache(_ =>
                    {
                        _.ConnectionString = sessionCs;
                        _.SchemaName = _config[ConfigurationKey.SqlSessionSchemaName] ?? "dbo";
                        _.TableName = sessionTable;
                    });
                    break;

                default:
                    services.AddDistributedMemoryCache();
                    break;
            }

            services.Configure<RouteOptions>(_ =>
                _.ConstraintMap.Add("cultureConstraint", typeof(CultureRouteConstraint)));

            // add MVC
            services.AddControllersWithViews(_ =>
                    _.ModelBinderProviders.RemoveType<DateTimeModelBinderProvider>())
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddSessionStateTempDataProvider()
                .AddDataAnnotationsLocalization(_ =>
                {
                    _.DataAnnotationLocalizerProvider = (__, factory)
                        => factory.Create(typeof(Resources.Shared));
                })
                .AddRazorRuntimeCompilation();

            services.AddWebOptimizer(_ =>
            {
                _.AddFiles("text/javascript", "/js/*");
                _.AddFiles("text/css", "/css/*");

                _.AddJavaScriptBundle("/js/main.min.js",
                    "js/jquery.js",
                    "js/jquery.validate.js",
                    "js/jquery.validate.unobtrusive.js",
                    "js/popper.js",
                    "js/bootstrap.js",
                    "js/moment.min.js",
                    "js/moment-timezone.min.js",
                    "js/tom-select.complete.js",
                    "Scripts/gra.js").UseContentRoot();
                // this tool cannot currently minify tempus-dominus.js
                _.AddJavaScriptBundle("/js/tempus-dominus.min.js",
                    new WebOptimizer.Processors.JsSettings
                    {
                        CodeSettings = new NUglify.JavaScript.CodeSettings { MinifyCode = false }
                    },
                    "js/tempus-dominus.min.js").UseContentRoot();
                _.AddJavaScriptBundle("/js/performerregistration.min.js",
                    "Scripts/performerregistration.js").UseContentRoot();
                _.AddJavaScriptBundle("/js/markdown.min.js",
                    "js/commonmark.js",
                    "Scripts/WMD.js").UseContentRoot();
                _.AddJavaScriptBundle("/js/qr-code-styling.min.js",
                    "js/qr-code-styling.js").UseContentRoot();
                _.AddJavaScriptBundle("/js/slick.min.js",
                    "js/slick.js").UseContentRoot();
                _.AddJavaScriptBundle("/js/slick-avatar.min.js",
                    "Scripts/slick-avatar.js").UseContentRoot();
                _.AddJavaScriptBundle("/js/jquery-ui.min.js",
                    "Scripts/jquery-ui-1.12.1.custom/jquery-ui.js").UseContentRoot();
                _.AddJavaScriptBundle("/js/messages_es.min.js",
                    "js/messages_es.js").UseContentRoot();

                _.AddCssBundle("/css/main.min.css",
                    "css/bootstrap.css",
                    "css/all.css",
                    "css/v4-shims.css",
                    "css/bootstrap-datetimepicker.css",
                    "css/tempus-dominus.css",
                    "css/tom-select.bootstrap5.css",
                    "Styles/gra.css").UseContentRoot();
                _.AddCssBundle("/css/missioncontrol.min.css",
                    "Styles/missioncontrol.css").UseContentRoot();
                _.AddCssBundle("/css/performerregistration.min.css",
                    "Styles/performerregistration.css").UseContentRoot();
                _.AddCssBundle("/css/performercoversheet.min.css",
                    "Styles/performercoversheet.css").UseContentRoot();
                _.AddCssBundle("/css/markdown.min.css",
                    "Styles/WMD.css").UseContentRoot();
                _.AddCssBundle("/css/slick.min.css",
                    "css/slick.css",
                    "css/slick-theme.css").UseContentRoot();
                _.AddCssBundle("/css/performerregistration.min.css",
                    "Styles/performerregistration.css").UseContentRoot();
                _.AddCssBundle("/css/jquery-ui.min.css",
                    "Scripts/jquery-ui-1.12.1.custom/jquery-ui.css").UseContentRoot();
            });

            // Add custom view directory
            services.Configure<RazorViewEngineOptions>(options =>
                options.ViewLocationFormats.Insert(0, "/shared/views/{1}/{0}.cshtml")
            );

            services.AddAntiforgery(_ => _.Cookie.Name = $"{discriminator}-af");

            // set cookie authentication options
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(_ =>
                {
                    _.LoginPath = "/SignIn";
                    _.LogoutPath = "/Home/Signout";
                    _.AccessDeniedPath = "/";
                    _.Cookie.IsEssential = true;
                    _.Cookie.Name = $"{discriminator}-app";
                    _.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                    _.Cookie.HttpOnly = true;
                });

            services.AddAuthorization(options =>
            {
                foreach (object permissionName in Enum.GetValues(typeof(Domain.Model.Permission)))
                {
                    options.AddPolicy(permissionName.ToString(),
                        _ => _.RequireClaim(ClaimType.Permission, permissionName.ToString()));
                }

                options.AddPolicy(Policy.ActivateChallenges,
                    _ => _.RequireClaim(ClaimType.Permission,
                        nameof(Permission.ActivateAllChallenges),
                        nameof(Permission.ActivateSystemChallenges)));
            });

            // service facades
            services.AddScoped<Controllers.ServiceFacade.Controller,
                Controllers.ServiceFacade.Controller>();
            services.AddScoped<Data.ServiceFacade.Repository, Data.ServiceFacade.Repository>();

            // database
            if (string.IsNullOrEmpty(_config[ConfigurationKey.ConnectionStringName]))
            {
                throw new GraFatalException("GraConnectionStringName is not configured in appsettings.json - cannot continue");
            }

            string csName = _config[ConfigurationKey.ConnectionStringName]
                ?? throw new GraFatalException($"{ConfigurationKey.ConnectionStringName} must be provided.");

            // configure ef errors to throw, log, or ignore as appropriate for the environment
            // see https://docs.microsoft.com/en-us/ef/core/querying/related-data#ignored-includes
            var throwEvents = new List<EventId>();
            var logEvents = new List<EventId>();
            var ignoreEvents = new List<EventId>();

            string cs = _config.GetConnectionString(csName)
                ?? throw new GraFatalException($"A {csName} connection string must be provided.");
            switch (_config[ConfigurationKey.ConnectionStringName])
            {
                case ConnectionStringNameSqlServer:
                    services.AddDbContextPool<Data.Context, Data.SqlServer.SqlServerContext>(
                        _ => _.UseSqlServer(cs)
                        .ConfigureWarnings(w => w
                            .Throw(throwEvents.ToArray())
                            .Log(logEvents.ToArray())
                            .Ignore(ignoreEvents.ToArray())));
                    services.AddHealthChecks();
                    break;

                case ConnectionStringNameSQLite:
                    services.AddDbContextPool<Data.Context, Data.SQLite.SQLiteContext>(
                        _ => _.UseSqlite(cs)
                            .ConfigureWarnings(w => w
                                .Throw(throwEvents.ToArray())
                                .Log(logEvents.ToArray())
                                .Ignore(ignoreEvents.ToArray())));
                    services.AddHealthChecks();
                    break;

                default:
                    throw new GraFatalException($"Unknown GraConnectionStringName: {csName}");
            }

            // store the data protection key in the database
            services.AddDataProtection().PersistKeysToDbContext<Data.Context>();

            // utilities
            services.AddScoped<ICodeGenerator, CodeGenerator>();
            services.AddScoped<ICodeSanitizer, CodeSanitizer>();
            services.AddScoped<IDateTimeProvider, CurrentDateTimeProvider>();
            services.AddScoped<IEntitySerializer, EntitySerializer>();
            services.AddScoped<IGraCache, Utility.GraCache>();
            services.AddScoped<IPasswordValidator, PasswordValidator>();
            services.AddScoped<IPathResolver, PathResolver>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IUserContextProvider, Controllers.UserContextProvider>();
            services.AddScoped<Security.Abstract.IPasswordHasher, Security.PasswordHasher>();
            services.AddScoped<WebSocketHandler>();

            // filters
            services.AddScoped<Controllers.Filter.MissionControlFilterAttribute>();
            services.AddScoped<Controllers.Filter.NotificationFilter>();
            services.AddScoped<Controllers.Filter.SessionTimeoutFilterAttribute>();
            services.AddScoped<Controllers.Filter.SiteFilterAttribute>();
            services.AddScoped<Controllers.Filter.UserFilter>();

            // services
            services.AddScoped<ActivityService>();
            services.AddScoped<AttachmentService>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<AuthorizationCodeService>();
            services.AddScoped<AvatarService>();
            services.AddScoped<BadgeService>();
            services.AddScoped<BranchImportExportService>();
            services.AddScoped<CarouselService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<ChallengeService>();
            services.AddScoped<DailyLiteracyTipService>();
            services.AddScoped<DashboardContentService>();
            services.AddScoped<DefaultItemsService>();
            services.AddScoped<Domain.Report.ServiceFacade.Report>();
            services.AddScoped<DrawingService>();
            services.AddScoped<EmailBulkService>();
            services.AddScoped<EmailManagementService>();
            services.AddScoped<EmailReminderService>();
            services.AddScoped<EmailService>();
            services.AddScoped<EventImportService>();
            services.AddScoped<EventService>();
            services.AddScoped<ExitLandingService>();
            services.AddScoped<GroupTypeService>();
            services.AddScoped<JobService>();
            services.AddScoped<JoinCodeService>();
            services.AddScoped<LanguageService>();
            services.AddScoped<MailService>();
            services.AddScoped<MessageTemplateService>();
            services.AddScoped<NewsService>();
            services.AddScoped<PageService>();
            services.AddScoped<PerformerSchedulingService>();
            services.AddScoped<PointTranslationService>();
            services.AddScoped<PrizeWinnerService>();
            services.AddScoped<QuestionnaireService>();
            services.AddScoped<ReportService>();
            services.AddScoped<ReportRequestService>();
            services.AddScoped<RoleService>();
            services.AddScoped<SampleDataService>();
            services.AddScoped<SchoolImportExportService>();
            services.AddScoped<SchoolService>();
            services.AddScoped<SegmentService>();
            services.AddScoped<SiteLookupService>();
            services.AddScoped<SiteService>();
            services.AddScoped<SocialManagementService>();
            services.AddScoped<SocialService>();
            services.AddScoped<SpatialService>();
            services.AddScoped<SystemInformationService>();
            services.AddScoped<TriggerService>();
            services.AddScoped<UserImportService>();
            services.AddScoped<UserService>();
            services.AddScoped<VendorCodeService>();

            // inject reports
            var reports = typeof(Domain.Report.Abstract.BaseReport)
                .Assembly
                .GetTypes()
                .Where(_ => _.IsClass
                    && !_.IsAbstract
                    && _.IsSubclassOf(typeof(Domain.Report.Abstract.BaseReport)));

            foreach (var reportType in reports)
            {
                services.AddScoped(reportType);
            }

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
                    throw new GraFatalException($"Unable to perform initial setup - unrecognized GraDefaultProgramSetup: {initialProgramSetup}");
            }

            // repositories
            services.AddScoped<Domain.Repository.IAnswerRepository, Data.Repository.AnswerRepository>();
            services.AddScoped<Domain.Repository.IAttachmentRepository, Data.Repository.AttachmentRepository>();
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
            services.AddScoped<Domain.Repository.IDirectEmailHistoryRepository, Data.Repository.DirectEmailHistoryRepository>();
            services.AddScoped<Domain.Repository.IDirectEmailTemplateRepository, Data.Repository.DirectEmailTemplateRepository>();
            services.AddScoped<Domain.Repository.IDrawingCriterionRepository, Data.Repository.DrawingCriterionRepository>();
            services.AddScoped<Domain.Repository.IDrawingRepository, Data.Repository.DrawingRepository>();
            services.AddScoped<Domain.Repository.IEmailBaseRepository, Data.Repository.EmailBaseRepository>();
            services.AddScoped<Domain.Repository.IEmailReminderRepository, Data.Repository.EmailReminderRepository>();
            services.AddScoped<Domain.Repository.IEmailSubscriptionAuditLogRepository, Data.Repository.EmailSubscriptionAuditLogRepository>();
            services.AddScoped<Domain.Repository.IEventRepository, Data.Repository.EventRepository>();
            services.AddScoped<Domain.Repository.IExitLandingMessagesRepository, Data.Repository.ExitLandingMessagesRepository>();
            services.AddScoped<Domain.Repository.IFeaturedChallengeGroupRepository, Data.Repository.FeaturedChallengeGroupRepository>();
            services.AddScoped<Domain.Repository.IGroupInfoRepository, Data.Repository.GroupInfoRepository>();
            services.AddScoped<Domain.Repository.IGroupTypeRepository, Data.Repository.GroupTypeRepository>();
            services.AddScoped<Domain.Repository.IJobRepository, Data.Repository.JobRepository>();
            services.AddScoped<Domain.Repository.IJoinCodeRepository, Data.Repository.JoinCodeRepository>();
            services.AddScoped<Domain.Repository.ILanguageRepository, Data.Repository.LanguageRepository>();
            services.AddScoped<Domain.Repository.ILocationRepository, Data.Repository.LocationRepository>();
            services.AddScoped<Domain.Repository.IMailRepository, Data.Repository.MailRepository>();
            services.AddScoped<Domain.Repository.IMessageTemplateRepository, Data.Repository.MessageTemplateRepository>();
            services.AddScoped<Domain.Repository.INewsCategoryRepository, Data.Repository.NewsCategoryRepository>();
            services.AddScoped<Domain.Repository.INewsPostRepository, Data.Repository.NewsPostRepository>();
            services.AddScoped<Domain.Repository.INotificationRepository, Data.Repository.NotificationRepository>();
            services.AddScoped<Domain.Repository.IPageHeaderRepository, Data.Repository.PageHeaderRepository>();
            services.AddScoped<Domain.Repository.IPageRepository, Data.Repository.PageRepository>();
            services.AddScoped<Domain.Repository.IPointTranslationRepository, Data.Repository.PointTranslationRepository>();
            services.AddScoped<Domain.Repository.IPrizeWinnerRepository, Data.Repository.PrizeWinnerRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.IPsAgeGroupRepository, Data.Repository.PsAgeGroupRepository>();
            services.AddScoped<Domain.Repository.IPsBlackoutDateRepository, Data.Repository.PsBlackoutDateRepository>();
            services.AddScoped<Domain.Repository.IPsBranchSelectionRepository, Data.Repository.PsBranchSelectionRepository>();
            services.AddScoped<Domain.Repository.IPsKitImageRepository, Data.Repository.PsKitImageRepository>();
            services.AddScoped<Domain.Repository.IPsKitRepository, Data.Repository.PsKitRepository>();
            services.AddScoped<Domain.Repository.IPsPerformerImageRepository, Data.Repository.PsPerformerImageRepository>();
            services.AddScoped<Domain.Repository.IPsPerformerRepository, Data.Repository.PsPerformerRepository>();
            services.AddScoped<Domain.Repository.IPsPerformerScheduleRepository, Data.Repository.PsPerformerScheduleRepository>();
            services.AddScoped<Domain.Repository.IPsProgramImageRepository, Data.Repository.PsProgramImageRepository>();
            services.AddScoped<Domain.Repository.IPsProgramRepository, Data.Repository.PsProgramRepository>();
            services.AddScoped<Domain.Repository.IPsSettingsRepository, Data.Repository.PsSettingsRepository>();
            services.AddScoped<Domain.Repository.IQuestionnaireRepository, Data.Repository.QuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IQuestionRepository, Data.Repository.QuestionRepository>();
            services.AddScoped<Domain.Repository.IRecoveryTokenRepository, Data.Repository.RecoveryTokenRepository>();
            services.AddScoped<Domain.Repository.IReportCriterionRepository, Data.Repository.ReportCriterionRepository>();
            services.AddScoped<Domain.Repository.IReportRequestRepository, Data.Repository.ReportRequestRepository>();
            services.AddScoped<Domain.Repository.IRequiredQuestionnaireRepository, Data.Repository.RequiredQuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IRoleRepository, Data.Repository.RoleRepository>();
            services.AddScoped<Domain.Repository.ISchoolDistrictRepository, Data.Repository.SchoolDistrictRepository>();
            services.AddScoped<Domain.Repository.ISchoolRepository, Data.Repository.SchoolRepository>();
            services.AddScoped<Domain.Repository.ISegmentRepository, Data.Repository.SegmentRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.ISiteSettingRepository, Data.Repository.SiteSettingRepository>();
            services.AddScoped<Domain.Repository.ISocialHeaderRepository, Data.Repository.SocialHeaderRepository>();
            services.AddScoped<Domain.Repository.ISocialRepository, Data.Repository.SocialRepository>();
            services.AddScoped<Domain.Repository.ISpatialDistanceRepository, Data.Repository.SpatialDistanceRepository>();
            services.AddScoped<Domain.Repository.ISystemInformationRepository, Data.Repository.SystemInformationRepository>();
            services.AddScoped<Domain.Repository.ISystemRepository, Data.Repository.SystemRepository>();
            services.AddScoped<Domain.Repository.ITriggerRepository, Data.Repository.TriggerRepository>();
            services.AddScoped<Domain.Repository.IUserLogRepository, Data.Repository.UserLogRepository>();
            services.AddScoped<Domain.Repository.IUserRepository, Data.Repository.UserRepository>();
            services.AddScoped<Domain.Repository.IVendorCodePackingSlipRepository, Data.Repository.VendorCodePackingSlipRepository>();
            services.AddScoped<Domain.Repository.IVendorCodeRepository, Data.Repository.VendorCodeRepository>();
            services.AddScoped<Domain.Repository.IVendorCodeTypeRepository, Data.Repository.VendorCodeTypeRepository>();

            services.AddMapster();
            MappingConfig.RegisterMappings();

            services.AddScoped<JobTaskRunner>();
            services.AddHostedService<JobBackgroundService>();
        }
    }
}
