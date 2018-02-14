using System;
using System.IO;
using System.Reflection;
using System.Linq;
using AutoMapper;
using GRA.Abstract;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Http;
using GRA.CommandLine.FakeWeb;
using GRA.CommandLine.Base;

namespace GRA.CommandLine
{
    class Program
    {
        private const string VersionSuffix = "-alpha9";
        public static int Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            bool developMode = !string.IsNullOrEmpty(config["ASPNETCORE_ENVIRONMENT"])
                && config["ASPNETCORE_ENVIRONMENT"].Equals("Development", StringComparison.CurrentCultureIgnoreCase);

            #region Initial setup of CommandLineApplication
            var app = new CommandLineApplication
            {
                Name = "gracl",
                FullName = "GRA command-line interface",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("--version",
                $"Version {PlatformServices.Default.Application.ApplicationVersion}{VersionSuffix}");

            // default option if no command is specified
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 2;
            });
            #endregion Initial setup of CommandLineApplication

            #region Default connection string settings in case we don't have a CS
            if (string.IsNullOrEmpty(config[ConfigurationKey.DefaultCSSqlServer]))
            {
                config[ConfigurationKey.DefaultCSSqlServer] = DefaultConnectionString.SqlServer;
            }
            if (string.IsNullOrEmpty(config[ConfigurationKey.DefaultCSSQLite]))
            {
                config[ConfigurationKey.DefaultCSSQLite] = DefaultConnectionString.SQLite;
            }
            #endregion Default connection string settings in case we don't have a CS

            // set up our Dependency Injection collection
            var services = new ServiceCollection();
            services.AddSingleton(_ => config);
            services.AddLogging();
            services.AddMemoryCache();
            services.AddSingleton<ConfigureUserSite>();
            services.AddSingleton<IHttpContextAccessor, FakeHttpContext>();
            services.AddSingleton<ServiceFacade>();
            services.AddSingleton<IDateTimeProvider, SettableDateTimeProvider>();

            // data generation
            services.AddSingleton<DataGenerator.Activity>();
            services.AddSingleton<DataGenerator.DateTime>();
            services.AddSingleton<DataGenerator.User>();

            #region Dependency Injection brought from GRA.Web
            /**************************************************************************************/
            // service facades
            services.AddScoped<Controllers.ServiceFacade.Controller, Controllers.ServiceFacade.Controller>();
            services.AddScoped<Data.ServiceFacade.Repository, Data.ServiceFacade.Repository>();

            // database
            services.AddScoped<Data.Context, Data.SqlServer.SqlServerContext>();
            //services.AddScoped<Data.Context, Data.SQLite.SQLiteContext>();

            // utilities
            services.AddScoped<IUserContextProvider, Controllers.UserContextProvider>();
            services.AddScoped<Security.Abstract.IPasswordHasher, Security.PasswordHasher>();
            services.AddScoped<Abstract.IPasswordValidator, PasswordValidator>();
            services.AddScoped<Abstract.IPathResolver, PathResolver>();
            services.AddScoped<Abstract.ITokenGenerator, TokenGenerator>();
            services.AddScoped<Abstract.IEntitySerializer, EntitySerializer>();
            services.AddScoped<Abstract.ICodeGenerator, CodeGenerator>();
            services.AddScoped<ICodeSanitizer, CodeSanitizer>();

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
            services.AddScoped<SystemInformationService>();
            services.AddScoped<TriggerService>();
            services.AddScoped<UserService>();
            services.AddScoped<VendorCodeService>();

            // service resolution
            services.AddScoped<IInitialSetupService, SetupMultipleProgramService>();

            // repositories
            services.AddScoped<Domain.Repository.IAnswerRepository, Data.Repository.AnswerRepository>();
            services.AddScoped<Domain.Repository.IAuthorizationCodeRepository, Data.Repository.AuthorizationCodeRepository>();
            services.AddScoped<Domain.Repository.IBadgeRepository, Data.Repository.BadgeRepository>();
            services.AddScoped<Domain.Repository.IBookRepository, Data.Repository.BookRepository>();
            services.AddScoped<Domain.Repository.IBranchRepository, Data.Repository.BranchRepository>();
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
            services.AddScoped<Domain.Repository.IEventRepository, Data.Repository.EventRepository>();
            services.AddScoped<Domain.Repository.ILocationRepository, Data.Repository.LocationRepository>();
            services.AddScoped<Domain.Repository.IMailRepository, Data.Repository.MailRepository>();
            services.AddScoped<Domain.Repository.INotificationRepository, Data.Repository.NotificationRepository>();
            services.AddScoped<Domain.Repository.IPageRepository, Data.Repository.PageRepository>();
            services.AddScoped<Domain.Repository.IPointTranslationRepository, Data.Repository.PointTranslationRepository>();
            services.AddScoped<Domain.Repository.IPrizeWinnerRepository, Data.Repository.PrizeWinnerRepository>();
            services.AddScoped<Domain.Repository.IProgramRepository, Data.Repository.ProgramRepository>();
            services.AddScoped<Domain.Repository.IRequiredQuestionnaireRepository, Data.Repository.RequiredQuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IQuestionRepository, Data.Repository.QuestionRepository>();
            services.AddScoped<Domain.Repository.IQuestionnaireRepository, Data.Repository.QuestionnaireRepository>();
            services.AddScoped<Domain.Repository.IRecoveryTokenRepository, Data.Repository.RecoveryTokenRepository>();
            services.AddScoped<Domain.Repository.IRoleRepository, Data.Repository.RoleRepository>();
            services.AddScoped<Domain.Repository.ISchoolDistrictRepository, Data.Repository.SchoolDistrictRepository>();
            services.AddScoped<Domain.Repository.ISchoolRepository, Data.Repository.SchoolRepository>();
            services.AddScoped<Domain.Repository.ISchoolTypeRepository, Data.Repository.SchoolTypeRepository>();
            services.AddScoped<Domain.Repository.ISiteRepository, Data.Repository.SiteRepository>();
            services.AddScoped<Domain.Repository.ISystemInformationRepository, Data.Repository.SystemInformationRepository>();
            services.AddScoped<Domain.Repository.ISystemRepository, Data.Repository.SystemRepository>();
            services.AddScoped<Domain.Repository.ITriggerRepository, Data.Repository.TriggerRepository>();
            services.AddScoped<Domain.Repository.IUserLogRepository, Data.Repository.UserLogRepository>();
            services.AddScoped<Domain.Repository.IUserRepository, Data.Repository.UserRepository>();
            services.AddScoped<Domain.Repository.IVendorCodeRepository, Data.Repository.VendorCodeRepository>();
            services.AddScoped<Domain.Repository.IVendorCodeTypeRepository, Data.Repository.VendorCodeTypeRepository>();
            /**************************************************************************************/
            #endregion Dependency Injection brought from GRA.Web

            services.AddAutoMapper();

            services.AddSingleton(_ => app);

            // loop through and add each ICommand to DI
            var commands = Assembly
                .GetEntryAssembly()
                .GetTypes()
                .Where(_ => typeof(BaseCommand).IsAssignableFrom(_)
                    && !_.GetTypeInfo().IsAbstract);

            foreach (var command in commands)
            {
                services.AddSingleton(command);
            }

            // all services added, build the DI service provider
            var serviceProvider = services.BuildServiceProvider();

            #region Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Error)
                .CreateLogger();

            serviceProvider.GetService<ILoggerFactory>()
                .AddSerilog();
            #endregion Configure Serilog

            // loop through commands to ensure they are all instantiated
            foreach (var command in commands)
            {
                var instantiated = serviceProvider.GetService(command);
            }

            // notate that we're done with initialization
            Log.Logger.Debug($"GRA command line initialization time: {sw.Elapsed}");

            // do the actual work
            int errorLevel = 1;
            try
            {
                errorLevel = app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                Log.Logger.Fatal(ex.Message);
                if (developMode)
                {
                    throw ex;
                }
            }

            sw.Stop();
            Log.Logger.Debug($"GRA command line total execution time: {sw.Elapsed}");

            // if we're in Visual Studio pause so the console box doesn't just disappear
            if (developMode)
            {
                Console.WriteLine("Running in development - hit enter to dismiss this window.");
                Console.ReadLine();
            }
            return errorLevel;
        }
    }
}