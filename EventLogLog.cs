using System;
using System.Diagnostics;
using Hangfire.Logging;

namespace WindowsService1
{
    public class EventLogLog : ILog
    {
        private readonly EventLog _eventLog;

        public EventLogLog(EventLog eventLog)
        {
            _eventLog = eventLog;
        }  

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null)
        {
            if (messageFunc == null) return logLevel >= LogLevel.Info;

            EventLogEntryType entryType = EventLogEntryType.Information;
            if (logLevel == LogLevel.Warn) entryType = EventLogEntryType.Warning;
            if (logLevel == LogLevel.Error || logLevel == LogLevel.Fatal) entryType = EventLogEntryType.Error;

            _eventLog.WriteEntry(messageFunc(), entryType);
            return true;
        }
    }
}