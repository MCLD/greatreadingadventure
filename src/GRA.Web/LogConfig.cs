using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.MSSqlServer;

namespace GRA.Web
{
    public static class LogConfig
    {
        private const string ErrorControllerName = "GRA.Controllers.ErrorController";

        public static LoggerConfiguration Build(IConfiguration config,
            LoggingLevelSwitch levelSwitch)
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
                    Version.GetShortVersion());

            string instance = config[ConfigurationKey.InstanceName];

            if (!string.IsNullOrEmpty(instance))
            {
                loggerConfig.Enrich.WithProperty(LoggingEnrichment.Instance, instance);
            }

            loggerConfig
                .Enrich
                .FromLogContext()
                .WriteTo
                .Console(formatProvider: CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(config[ConfigurationKey.SuppressTextLogs]))
            {
                string rollingLogLocation
                    = Path.Combine("shared", config[ConfigurationKey.RollingLogPath]);
                if (!string.IsNullOrEmpty(rollingLogLocation))
                {
                    string rollingLogFile = !string.IsNullOrEmpty(instance)
                        ? Path.Combine(rollingLogLocation, $"log-{instance}-{{Date}}.txt")
                        : Path.Combine(rollingLogLocation, "log-{Date}.txt");

                    loggerConfig.WriteTo.Logger(_ => _
                        .Filter
                        .ByExcluding(Matching.FromSource(ErrorControllerName))
                        .WriteTo
                        .RollingFile(rollingLogFile, formatProvider: CultureInfo.InvariantCulture));

                    string httpErrorFileTag = config[ConfigurationKey.RollingLogHttp];
                    if (!string.IsNullOrEmpty(httpErrorFileTag))
                    {
                        string httpLogFile = !string.IsNullOrEmpty(instance)
                            ? Path.Combine(rollingLogLocation,
                                $"{httpErrorFileTag}-{instance}-{{Date}}.txt")
                            : Path.Combine(rollingLogLocation,
                                $"{httpErrorFileTag}-{{Date}}.txt");

                        loggerConfig.WriteTo.Logger(_ => _
                            .Filter
                            .ByIncludingOnly(Matching.FromSource(ErrorControllerName))
                            .WriteTo
                            .RollingFile(httpLogFile,
                                formatProvider: CultureInfo.InvariantCulture));
                    }
                }
            }

            string sqlLog = config.GetConnectionString(ConfigurationKey.CSSqlServerSerilog);
            if (!string.IsNullOrEmpty(sqlLog))
            {
                loggerConfig
                    .WriteTo.MSSqlServer(sqlLog,
                        new MSSqlServerSinkOptions
                        {
                            TableName = "Logs",
                            AutoCreateSqlTable = true
                        },
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        formatProvider: CultureInfo.InvariantCulture,
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
                                new SqlColumn(LoggingEnrichment.Instance,
                                    SqlDbType.NVarChar,
                                    dataLength: 255),
                                new SqlColumn(LoggingEnrichment.RemoteAddress,
                                    SqlDbType.NVarChar,
                                    dataLength: 255)
                            }
                        });
            }

            if (!string.IsNullOrEmpty(config[ConfigurationKey.SeqEndpoint]))
            {
                var apiKey = config[ConfigurationKey.SeqApiKey];

                if (string.IsNullOrEmpty(apiKey))
                {
                    loggerConfig.WriteTo.Seq(config[ConfigurationKey.SeqEndpoint]);
                }
                else
                {
                    loggerConfig.WriteTo.Seq(config[ConfigurationKey.SeqEndpoint],
                        apiKey: config[ConfigurationKey.SeqApiKey],
                        controlLevelSwitch: levelSwitch);
                }
            }

            return loggerConfig;
        }
    }
}
