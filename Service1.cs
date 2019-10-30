using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Server;

namespace WindowsService1
{
    public partial class HangfireService : ServiceBase
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);

        private List<IBackgroundProcessingServer> _servers = new List<IBackgroundProcessingServer>();
        private BackgroundJobServerOptions _options;

        public HangfireService()
        {
            InitializeComponent();
            eventLog1 = new EventLog("Application");
            eventLog1.Source = "Application";

            GlobalConfiguration.Configuration
                .UseLogProvider(new EventLogLogProvider(eventLog1))
                .UseMemoryStorage();
        }

        protected override void OnStart(string[] args)
        {
            //BackgroundJob.Enqueue(() => MyWork.LongRunning(CancellationToken.None));

            _options = new BackgroundJobServerOptions
            {
                Queues = new[] { "default" },
                WorkerCount = Environment.ProcessorCount
            };

            // We need to override these properties to implement
            // our own WaitForShutdown logic to prevent paused
            // servers from stopping earlier than required.
            _options.StopTimeout = Timeout.InfiniteTimeSpan;
            _options.ShutdownTimeout = Timeout.InfiniteTimeSpan;

            StartNewServer();
        }

        protected override void OnStop()
        {
            StopCurrentServer();

            foreach (var server in _servers)
            {
                while (!server.WaitForShutdown(DefaultTimeout))
                {
                    RequestAdditionalTime((int)DefaultTimeout.TotalMilliseconds);
                }
            }
        }

        protected override void OnContinue()
        {
            CleanUp();
            StartNewServer();
        }

        protected override void OnPause()
        {
            CleanUp();
            StopCurrentServer();
        }

        private void StartNewServer()
        {
            _servers.Add(new BackgroundJobServer(_options));
        }

        private void StopCurrentServer()
        {
            var current = _servers.Last();
            current?.SendStop();
        }

        private void CleanUp()
        {
            _servers = _servers.Where(x => !x.WaitForShutdown(TimeSpan.Zero)).ToList();
        }
    }
}
