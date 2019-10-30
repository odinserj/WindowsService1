using System;
using System.Threading;
using Hangfire.Logging;

namespace WindowsService1
{
    public class MyWork
    {
        public static void LongRunning(CancellationToken token)
        {
            var logger = LogProvider.GetCurrentClassLogger();

            while (!token.IsCancellationRequested)
            {
                logger.Info("Performing the work...");
                token.WaitHandle.WaitOne(TimeSpan.FromSeconds(30));
            }

            logger.Info("Finished or shutting down!");
        }
    }
}