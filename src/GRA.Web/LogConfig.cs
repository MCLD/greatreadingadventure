using System.Data;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.MSSqlServer;

namespace GRA.Web
{
    public class LogConfig
    {
        private const string ErrorControllerName = "GRA.Controllers.ErrorController";

        public const string ApplicationEnrichment = "Application";
        public const string VersionEnrichment = "Version";
        public const string IdentifierEnrichment = "Identifier";
        public const string InstanceEnrichment = "Instance";
        public const string RemoteAddressEnrichment = "RemoteAddress";

        public LoggerConfiguration Build(IConfiguration config)
        {
            LoggerConfiguration loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.WithProperty(ApplicationEnrichment,
                    Assembly.GetExecutingAssembly().GetName().Name)
                .Enrich.WithProperty(VersionEnrichment, new Version().GetShortVersion())
                .Enrich.FromLogContext()
                .WriteTo.Console();

            string instance = config[ConfigurationKey.InstanceName];

            if (!string.IsNullOrEmpty(instance))
            {
                loggerConfig.Enrich.WithProperty(InstanceEnrichment, instance);
            }

            string rollingLogLocation
                = Path.Combine("shared", config[ConfigurationKey.RollingLogPath]);
            if (!string.IsNullOrEmpty(rollingLogLocation))
            {
                string rollingLogFile = !string.IsNullOrEmpty(instance)
                    ? Path.Combine(rollingLogLocation, $"log-{instance}-.txt")
                    : Path.Combine(rollingLogLocation, "log-.txt");

                loggerConfig.WriteTo.Logger(_ => _
                    .Filter.ByExcluding(Matching.FromSource(ErrorControllerName))
                    .WriteTo.File(rollingLogFile, rollingInterval: RollingInterval.Day));

                string httpErrorFileTag = config[ConfigurationKey.RollingLogHttp];
                if (!string.IsNullOrEmpty(httpErrorFileTag))
                {
                    string httpLogFile = !string.IsNullOrEmpty(instance)
                        ? Path.Combine(rollingLogLocation, $"{httpErrorFileTag}-{instance}-.txt")
                        : Path.Combine(rollingLogLocation + $"{httpErrorFileTag}-.txt");

                    loggerConfig.WriteTo.Logger(_ => _
                        .Filter.ByIncludingOnly(Matching.FromSource(ErrorControllerName))
                        .WriteTo.File(httpLogFile, rollingInterval: RollingInterval.Day));
                }
            }

            string sqlLog = config.GetConnectionString(ConfigurationKey.CSSqlServerSerilog);
            if (!string.IsNullOrEmpty(sqlLog))
            {
                loggerConfig
                    .WriteTo.Logger(_ => _
                    .Filter.ByExcluding(Matching.FromSource(ErrorControllerName))
                    .WriteTo.MSSqlServer(sqlLog,
                        "Logs",
                        autoCreateSqlTable: true,
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        columnOptions: new ColumnOptions
                        {
                            AdditionalDataColumns = new DataColumn[]
                            {
                                new DataColumn(ApplicationEnrichment, typeof(string)) { MaxLength = 255 },
                                new DataColumn(VersionEnrichment, typeof(string)) { MaxLength = 255 },
                                new DataColumn(IdentifierEnrichment, typeof(string)) { MaxLength = 255 },
                                new DataColumn(InstanceEnrichment, typeof(string)) { MaxLength = 255 },
                                new DataColumn(RemoteAddressEnrichment, typeof(string)) { MaxLength = 255 }
                            }
                        }));
            }

            return loggerConfig;
        }
    }
}
