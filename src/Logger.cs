using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Paksys.NetCore.Logging
{
    public class Logger : ILogger
    {
        /* Sample Usage:
         *   var logFolderPath = @"C:\Users\khali\source\repos\WebApplication6\ConsoleApp1\Logs";
         *   var logger = new Logger(logFolderPath);
         *
         *   var ld = GetLogDetail("starting application", null);
         *
         *   var perfTracker = new PerfTracker(logFolderPath, ld);
         *
         *   logger.Log.Information("Hello, world!");
         *
         *   int a = 10, b = 0;
         *   try
         *   {
         *       logger.Log.Debug("Dividing {A} by {B}", a, b);
         *       Console.WriteLine(a / b);
         *   }
         *   catch (Exception ex)
         *   {
         *       logger.Log.Error(ex, "Something went wrong");
         *   }
         *
         *   logger.Verbose(ld);
         *
         *   logger.Info(ld);
         *   logger.Debug(ld);
         *
         *   logger.Warn(ld);
         *   logger.Fatal(ld);
         *  
         *
         *   perfTracker.Stop();
         *   
         *   logger.Usage(ld);
         *
         */

        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
        private string _outputTemplate;
        private readonly string _logFolderPath;

        public string OutputTemplate
        {
            get => string.IsNullOrWhiteSpace(_outputTemplate) ? DefaultOutputTemplate : _outputTemplate;
            set => _outputTemplate = value;
        }
        public Serilog.ILogger Log { get; set; }

        public Logger(string logFolderPath)
        {
            _logFolderPath = string.IsNullOrWhiteSpace(logFolderPath) 
                                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Logs\") 
                                : logFolderPath;

            Log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ColoredConsole()

                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Verbose)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "verbose_log-{Date}.txt"),
                        outputTemplate: OutputTemplate))

                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Debug)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "debug_log-{Date}.txt"),
                        outputTemplate: OutputTemplate))

                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "info_log-{Date}.txt"),
                        outputTemplate: OutputTemplate))

                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "warn_log-{Date}.txt"),
                        outputTemplate: OutputTemplate))

                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Error)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "error_log-{Date}.txt"),
                        outputTemplate: OutputTemplate))

                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Fatal)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "fatal_log-{Date}.txt"),
                        outputTemplate: OutputTemplate))
                
                .CreateLogger();
        }

        /*
         * Usage: [https://github.com/serilog/serilog-settings-configuration]
         *  A Serilog settings provider that reads from Microsoft.Extensions.Configuration, .NET Core's appsettings.json file.
         *  Configuration is read from the Serilog section.
         *
         *       {
         *         "Serilog": {
         *           "Using":  ["Serilog.Sinks.Console"],
         *           "MinimumLevel": "Debug",
         *           "WriteTo": [
         *             { "Name": "Console" },
         *             { "Name": "File", "Args": { "path": "%TEMP%\\Logs\\serilog-configuration-sample.txt" } }
         *           ],
         *           "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"],
         *           "Destructure": [
         *             { "Name": "With", "Args": { "policy": "Sample.CustomPolicy, Sample" } },
         *             { "Name": "ToMaximumDepth", "Args": { "maximumDestructuringDepth": 4 } },
         *             { "Name": "ToMaximumStringLength", "Args": { "maximumStringLength": 100 } },
         *             { "Name": "ToMaximumCollectionCount", "Args": { "maximumCollectionCount": 10 } }
         *           ],
         *           "Properties": {
		 *               "Application": "Sample"
         *           }
         *         }
         *       }
         *
         */
        public Logger(IConfiguration configuration)
        {
            Log = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }


        public void Perf(LogDetail logDetail)
        {
            using (var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "perf_log-{Date}.txt"),
                        outputTemplate: OutputTemplate)).CreateLogger())
            {
                log.Write(LogEventLevel.Information, "{@LogDetail}", logDetail);
            }
        }

        public void Usage(LogDetail logDetail)
        {
            using (var log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Logger(x => x.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Information)
                    .WriteTo.RollingFile(Path.Combine(_logFolderPath, "usage_log-{Date}.txt"),
                        outputTemplate: OutputTemplate)).CreateLogger())
            {
                log.Write(LogEventLevel.Information, "{@LogDetail}", logDetail);
            }
        }

        public void Verbose(LogDetail logDetail)
        {
            Log.Verbose("{@LogDetail}", logDetail);
        }

        public void Debug(LogDetail logDetail)
        {
            Log.Debug("{@LogDetail}", logDetail);
        }

        public void Info<T>(T propertyValue)
        {
            Log.Information("{@T}", propertyValue);
        }

        public void Warn(LogDetail logDetail)
        {
            Log.Warning("{@LogDetail}", logDetail);
        }

        public void Error(LogDetail logDetail)
        {
            if (logDetail.Exception != null)
            {
                logDetail.Message = GetMessageFromException(logDetail.Exception);
            }
            Log.Error("{@LogDetail}", logDetail);
        }

        public void Fatal(LogDetail logDetail)
        {
            if (logDetail.Exception != null)
            {
                logDetail.Message = GetMessageFromException(logDetail.Exception);
            }
            Log.Fatal("{@LogDetail}", logDetail);
        }


        private static string GetMessageFromException(Exception ex)
        {
            return (ex.InnerException != null) 
                ? GetMessageFromException(ex.InnerException) 
                : ex.Message;
        }
    }
}
