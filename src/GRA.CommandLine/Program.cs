﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using GRA.Abstract;
using GRA.CommandLine.Base;
using GRA.CommandLine.FakeWeb;
using GRA.Domain.Service;
using GRA.Domain.Service.Abstract;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GRA.CommandLine
{
    internal static class Program
    {
        private const string VersionSuffix = "-alpha1";

        public static int Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var builder = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            bool developMode = !string.IsNullOrEmpty(config["ASPNETCORE_ENVIRONMENT"])
                && config["ASPNETCORE_ENVIRONMENT"].Equals("Development", StringComparison.OrdinalIgnoreCase);

            #region Initial setup of CommandLineApplication
            var app = new CommandLineApplication
            {
                Name = "gracl",
                FullName = "GRA command-line interface",
            };
            app.HelpOption("-?|-h|--help");
            app.VersionOption("--version",
                $"Version {Assembly.GetEntryAssembly().GetName().Version}{VersionSuffix}");

            // default option if no command is specified
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 2;
            });
            #endregion Initial setup of CommandLineApplication

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
            services.AddScoped<Controllers.Filter.SiteFilterAttribute>();
            services.AddScoped<Controllers.Filter.UserFilter>();

            // services
            services.AddScoped<ActivityService>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<AvatarService>();
            services.AddScoped<BadgeService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<ChallengeService>();
            services.AddScoped<DrawingService>();
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
            services.AddScoped<SchoolImportExportService>();
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
            services.AddScoped<Domain.Repository.IAvatarBundleRepository, Data.Repository.AvatarBundleRepository>();
            services.AddScoped<Domain.Repository.IAvatarColorRepository, Data.Repository.AvatarColorRepository>();
            services.AddScoped<Domain.Repository.IAvatarElementRepository, Data.Repository.AvatarElementRepository>();
            services.AddScoped<Domain.Repository.IAvatarItemRepository, Data.Repository.AvatarItemRepository>();
            services.AddScoped<Domain.Repository.IAvatarLayerRepository, Data.Repository.AvatarLayerRepository>();
            services.AddScoped<Domain.Repository.IBadgeRepository, Data.Repository.BadgeRepository>();
            services.AddScoped<Domain.Repository.IBookRepository, Data.Repository.BookRepository>();
            services.AddScoped<Domain.Repository.IBranchRepository, Data.Repository.BranchRepository>();
            services.AddScoped<Domain.Repository.IDrawingCriterionRepository, Data.Repository.DrawingCriterionRepository>();
            services.AddScoped<Domain.Repository.IDrawingRepository, Data.Repository.DrawingRepository>();
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

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
                serviceProvider.GetService(command);
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
                    throw;
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