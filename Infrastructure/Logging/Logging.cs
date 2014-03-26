using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InfraCSharp.Infrastructure.Logging
{
    /// <summary>
    /// Creates a Logging class based on the log4net library
    /// </summary>
    /// <remarks>
    /// An Example Configuration File that assumes a folder called "Log" in the application root
    /// <![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <configuration>
    ///   <configSections>
    ///     <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler" />
    ///   </configSections>
    ///   <log4net>
    ///     <appender name="LogFileAppender" type="log4net.Appender.RollingFileAppender">
    /// 
    ///       <file value=".\\Log\\output" />
    ///       <param name="DatePattern" value="'.'yyyy-MM-dd'.log'"/>
    ///       
    ///       <param name="RollingStyle" value="Date"/>
    ///       <param name="StaticLogFileName" value="false"/>
    ///       
    ///       <maximumFileSize value="100KB" />
    ///       <appendToFile value="true" />
    ///       <maxSizeRollBackups value="2" />
    ///       <lockingModel type="log4net.Appender.FileAppender+MinimalLock"/>
    ///       
    ///       <layout type="log4net.Layout.PatternLayout">
    ///         <param name="ConversionPattern" value="%d %-5p %t | %m%n" />
    ///       </layout>
    ///     </appender>
    ///     
    ///     <root>
    ///       <level value="ALL" />
    ///       <appender-ref ref="LogFileAppender" />
    ///     </root>
    ///   </log4net>
    /// </configuration>
    /// ]]>
    /// </remarks>
    public class Logging
    {
        protected ILog baseLogger;
        protected FileInfo logFile;
        protected String logName;

        /// <summary>
        /// Creates a logger with the log configuration assumed to be located in the <code>Environment.CurrentDirectory</code>
        /// </summary>
        /// <param name="logConfigurationName">The configuration name of the logger, excluding the '.config' file extension</param>
        public Logging(string logConfigurationName)
            : this(Environment.CurrentDirectory, logConfigurationName) 
        { }

        /// <summary>
        /// Creates a logger with the log configuration in the specified directory.
        /// </summary>
        /// <remarks>Assumes a "logDirectory + logConfigurationName + .config" arrangement</remarks>
        /// <param name="logDirectory">The directory path to the logger config file</param>
        /// <param name="logConfigurationName">The configuration name of the logger, excluding the '.config' file extension</param>
        public Logging(string logDirectory, string logConfigurationName)
        {
            // Check that the log directory exists
            DirectoryInfo logParentDir = new DirectoryInfo(logDirectory);
            if (!logParentDir.Exists)
            {
                throw new DirectoryNotFoundException("Specified directory does not exist for the configuration file! Directory Could Not Be Found: " + logParentDir.FullName);
            }
            
            // Check that the log file exists
            this.logFile = new FileInfo(logParentDir.FullName + "\\" + logConfigurationName + ".config");
            if (!this.logFile.Exists)
            {
                throw new FileNotFoundException("Specified configuration file cannot be found.", logFile.FullName);
            }
            
            this.baseLogger = LogManager.GetLogger(typeof(Logging));
            this.logName = logConfigurationName;
            InitializeLogger();
        }

        /// <summary>
        /// Initializes the logger based on the requirement that all instance variables are set (not null)
        /// </summary>
        protected void InitializeLogger()
        {
            if (this.logFile == null ||
                this.logName == null ||
                this.baseLogger == null)
            {
                throw new Exception("Error while calling InitializeLogger() because one of the logFile, LogName or baseLogger is null. Please ensure the configuration file is properly configured.");
            }

            XmlConfigurator.Configure(logFile);
        }

        /// <summary>
        /// Appends a string to the configured log file based on the mode mask
        /// </summary>
        /// <param name="appendMessage">The string message that will be appended</param>
        /// <param name="mode">The mode mask for the message</param>
        protected void append(string appendMessage, int mode)
        {
            switch (mode)
            {
                case 1:  // Debug
                    baseLogger.Debug(appendMessage);
                    break;
                case 2:  // Fatal
                    baseLogger.Fatal(appendMessage);
                    break;
                case 3:  // Warn
                    baseLogger.Warn(appendMessage);
                    break;
                case 4:  // Error
                    baseLogger.Error(appendMessage);
                    break;
                case 5:  // Info
                default:
                    baseLogger.Info(appendMessage);
                    break;
            }
        }

        /// <summary>
        /// Append a Debug string to the log file.
        /// </summary>
        /// <param name="message">Debug String to append</param>
        public void Debug(string message, params object[] args)
        {
            append(string.Format(message, args), 1);
        }

        /// <summary>
        /// Append a Fatal string to the log file.
        /// </summary>
        /// <param name="message">Fatal String to append</param>
        public void Fatal(string message, params object[] args)
        {
            append(string.Format(message, args), 2);
        }

        /// <summary>
        /// Append a Warn string to the log file.
        /// </summary>
        /// <param name="message">Warn String to append</param>
        public void Warn(string message, params object[] args)
        {
            append(string.Format(message, args), 3);
        }

        /// <summary>
        /// Append an Error string to the log file.
        /// </summary>
        /// <param name="message">Error String to append</param>
        public void Error(string message, params object[] args)
        {
            append(string.Format(message, args), 4);
        }

        /// <summary>
        /// Appends an Error stacktrace as well as the message
        /// </summary>
        /// <param name="message">Error String to append</param>
        /// <param name="ex">Exception to serialize into log format</param>
        public void Error(string message, Exception ex)
        {
            Error(message);
            Error(ex.GetBaseException().Message);
            Error(ex.Message);
            Error(ex.TargetSite.Name);
            Error(ex.StackTrace);
        }

        /// <summary>
        /// Append an Info string to the log file.
        /// </summary>
        /// <param name="message">Info String to append</param>
        public void Info(string message, params object[] args)
        {
            append(string.Format(message, args), 5);
        }

        /// <summary>
        /// Clears the log file from any entries
        /// </summary>
        public void Clear()
        {
            try
            {
                FileAppender fAppender = (FileAppender)((Hierarchy)LogManager.GetRepository()).Root.Appenders[0];
                FileInfo logFile = new FileInfo(fAppender.File);

                // Close our output and delete the log file
                fAppender.Close();
                logFile.Delete();

                // Initialize the logger again
                InitializeLogger();

                LogStartMarker();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while attempting to access the zeroth appender to Clear() the log file.", ex);
            }
        }

        /// <summary>
        /// Puts a start marker in the log file that consists of dashes under the 'INFO' mask
        /// </summary>
        public void LogStartMarker() {
            LogStartMarker("-----------------------------------------");
        }

        /// <summary>
        /// Puts a start marker in the log file under the 'INFO' mask
        /// </summary>
        /// <param name="marker">A string that will be used as a marker</param>
        public void LogStartMarker(string marker)
        {
            Info(marker);
        }

        /// <summary>
        /// Puts a end marker in the log file that consists of a short message and equal signs under the 'INFO' mask
        /// </summary>
        public void LogEndMarker()
        {
            Info("");
            Info(string.Concat("Stopping:", this.logName));
            LogEndMarker("=========================================");
        }

        /// <summary>
        /// Puts an end marker in the log file under the 'INFO' mask
        /// </summary>
        /// <param name="marker">A string that will be used as a marker</param>
        public void LogEndMarker(string marker)
        {
            Info(marker);
        }
    }
}

