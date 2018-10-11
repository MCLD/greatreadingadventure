using System.Data;
using System.Diagnostics;
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

        private const string ApplicationEnrichment = "Application";
        private const string VersionEnrichment = "Version";
        private const string IdentifierEnrichment = "Identifier";
        private const string InstanceEnrichment = "Instance";
        private const string RemoteAddressEnrichment = "RemoteAddress";

        public LoggerConfiguration Build(IConfiguration config, string instance, string[] args)
        {
            var applicationName = Assembly.GetExecutingAssembly().GetName().Name;
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            LoggerConfiguration loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.WithProperty(ApplicationEnrichment, applicationName)
                .Enrich.WithProperty(VersionEnrichment, version)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            string rollingLogLocation = config[ConfigurationKey.RollingLogPath];
            if (!string.IsNullOrEmpty(rollingLogLocation))
            {
                if (!rollingLogLocation.EndsWith(Path.DirectorySeparatorChar))
                {
                    rollingLogLocation += Path.DirectorySeparatorChar;
                }
                rollingLogLocation += "log-";

                string rollingLogFile = rollingLogLocation + instance + "-{Date}.txt";

                loggerConfig.WriteTo.Logger(_ => _
                    .Filter.ByExcluding(Matching.FromSource(ErrorControllerName))
                    .WriteTo.RollingFile(rollingLogFile));

                string httpErrorFileTag = config[ConfigurationKey.RollingLogHttp];
                if (!string.IsNullOrEmpty(httpErrorFileTag))
                {
                    string httpLogFile = rollingLogLocation
                        + instance
                        + "-"
                        + httpErrorFileTag
                        + "-{Date}.txt";

                    loggerConfig.WriteTo.Logger(_ => _
                        .Filter.ByIncludingOnly(Matching.FromSource(ErrorControllerName))
                        .WriteTo.RollingFile(httpLogFile));
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
