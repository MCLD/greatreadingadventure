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
    public static class LogConfig
    {
        private const string ErrorControllerName = "GRA.Controllers.ErrorController";

        public static LoggerConfiguration Build(IConfiguration config)
        {
            if (config == null)
            {
                throw new System.ArgumentNullException(nameof(config));
            }

            LoggerConfiguration loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.WithProperty(LoggingEnrichment.Application,
                    Assembly.GetExecutingAssembly().GetName().Name)
                .Enrich.WithProperty(LoggingEnrichment.Version,
                    new Version().GetShortVersion());

            string instance = config[ConfigurationKey.InstanceName];

            if (!string.IsNullOrEmpty(instance))
            {
                loggerConfig.Enrich.WithProperty(LoggingEnrichment.Instance, instance);
            }

            loggerConfig.Enrich.FromLogContext()
                .WriteTo.Console();

            string rollingLogLocation
                = Path.Combine("shared", config[ConfigurationKey.RollingLogPath]);
            if (!string.IsNullOrEmpty(rollingLogLocation))
            {
                string rollingLogFile = !string.IsNullOrEmpty(instance)
                    ? Path.Combine(rollingLogLocation, $"log-{instance}-{{Date}}.txt")
                    : Path.Combine(rollingLogLocation, "log-{Date}.txt");

                loggerConfig.WriteTo.Logger(_ => _
                    .Filter.ByExcluding(Matching.FromSource(ErrorControllerName))
                    .WriteTo.RollingFile(rollingLogFile));

                string httpErrorFileTag = config[ConfigurationKey.RollingLogHttp];
                if (!string.IsNullOrEmpty(httpErrorFileTag))
                {
                    string httpLogFile = !string.IsNullOrEmpty(instance)
                        ? Path.Combine(rollingLogLocation, $"{httpErrorFileTag}-{instance}-{{Date}}.txt")
                        : Path.Combine(rollingLogLocation + $"{httpErrorFileTag}-{{Date}}.txt");

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
                            AdditionalColumns = new[]
                            {
                                new SqlColumn(LoggingEnrichment.Application,
                                    SqlDbType.NVarChar,
                                    dataLength: 255),
                                new SqlColumn(LoggingEnrichment.Version,
                                    SqlDbType.NVarChar,
                                    dataLength: 255),
                                new SqlColumn(LoggingEnrichment.Identifier,
                                    SqlDbType.NVarChar,
                                    dataLength: 255),
                                new SqlColumn(LoggingEnrichment.Instance,
                                    SqlDbType.NVarChar,
                                    dataLength: 255),
                                new SqlColumn(LoggingEnrichment.RemoteAddress,
                                    SqlDbType.NVarChar,
                                    dataLength: 255)
                            }
                        }));
            }

            if (!string.IsNullOrEmpty(config[ConfigurationKey.SeqEndpoint]))
            {
                loggerConfig.WriteTo.Logger(_ => _
                    .Filter.ByExcluding(Matching.FromSource(ErrorControllerName))
                    .WriteTo.Seq(config[ConfigurationKey.SeqEndpoint],
                        apiKey: config[ConfigurationKey.SeqApiKey]));
            }

            return loggerConfig;
        }
    }
}
