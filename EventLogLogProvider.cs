using System.Diagnostics;
using Hangfire.Logging;

namespace WindowsService1
{
    public class EventLogLogProvider : ILogProvider
    {
        private readonly EventLog _eventLog;

        public EventLogLogProvider(EventLog eventLog)
        {
            _eventLog = eventLog;
        }

        public ILog GetLogger(string name)
        {
            return new EventLogLog(_eventLog);
        }
    }
}