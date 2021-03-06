using System;
using System.Diagnostics;
using System.Text;

namespace Ably
{
    /// <summary>Level of a log message.</summary>
    public enum LogLevel : byte
    {
        Error = 0,
        Warning = 1,
        Info = 2,
        Debug = 3,
    }

    /// <summary>An interface that actually logs that messages somewhere.</summary>
    public interface ILoggerSink
    {
        void LogEvent( LogLevel level, string message );
    }

    /// <summary>The default logger implementation, that writes to debug output.</summary>
    internal class DefaultLoggerSink : ILoggerSink
    {
        readonly object syncRoot = new object();

        public void LogEvent( LogLevel level, string message )
        {
            lock ( syncRoot )
            {
                Debug.WriteLine( "Ably: [{0}] {1}", level, message );
            }
        }
    }

    /// <summary>An utility class for logging various messages.</summary>
    public static class Logger
    {
        /// <summary>Maximum level to log.</summary>
        /// <remarks>E.g. set to LogLevel.Warning to have only errors and warnings in the log.</remarks>
        public static LogLevel logLevel { get; set; }
        static ILoggerSink impl;

        static Logger()
        {
            logLevel = Config.DefaultLogLevel;
            impl = new DefaultLoggerSink();
        }

        /// <summary>Override logging destination.</summary>
        /// <param name="i">The new destination where to log thise messages. Pass null to disable logging completely.</param>
        /// <remarks>This method is mainly for unit tests. However, if in your product you're using some logging infrastructure like nlog or custom logging,
        /// you may want to provide your own <see cref="ILoggerSink"/> implementation to have messages from Ably logged along with your own ones.</remarks>
        public static void SetDestination( ILoggerSink i )
        {
            impl = i;
        }

        static void Message( LogLevel level, string message, params object[] args )
        {
            ILoggerSink i = impl;
            if( level > logLevel || null == i )
                return;
            i.LogEvent( level, string.Format( message, args ) );
        }

        /// <summary>Log an error message.</summary>
        internal static void Error( string message, Exception ex )
        {
            Message( LogLevel.Error, "{0} {1}", message, GetExceptionDetails( ex ) );
        }

        /// <summary>Log an error message.</summary>
        internal static void Error( string message, params object[] args )
        {
            Message( LogLevel.Error, message, args );
        }

        /// <summary>Log an informational message.</summary>
        internal static void Info( string message, params object[] args )
        {
            Message( LogLevel.Info, message, args );
        }

        /// <summary>Log a debug message.</summary>
        internal static void Debug( string message, params object[] args )
        {
            Message( LogLevel.Debug, message, args );
        }

        /// <summary>Produce long multiline string with the details about the exception, including inner exceptions, if any.</summary>
        static string GetExceptionDetails( Exception ex )
        {
            var message = new StringBuilder();
            var webException = ex as AblyException;
            if( webException != null )
            {
                message.AppendLine( "Error code: " + webException.ErrorInfo.Code );
                message.AppendLine( "Status code: " + webException.ErrorInfo.StatusCode );
                message.AppendLine( "Reason: " + webException.ErrorInfo.Reason );
            }

            message.AppendLine( ex.Message );
            message.AppendLine( ex.StackTrace );
            if( ex.InnerException != null )
            {
                message.AppendLine( "Inner exception:" );
                message.AppendLine( GetExceptionDetails( ex.InnerException ) );
            }
            return message.ToString();
        }
    }
}