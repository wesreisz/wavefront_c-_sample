using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Reporting.Wavefront.Builder;
using App.Metrics.Scheduling;
using Wavefront.SDK.CSharp.Common;
using Wavefront.SDK.CSharp.Proxy;
using Wavefront.SDK.CSharp.DirectIngestion;

namespace Wavefront_Proxy_Reporter
{
    class Program
    {
        //Wavefront proxy 
        static string proxyHost = "myproxy.vmware.com";
        static int metricsPort = 2878;

        //Direct Injestion
        static string wavefrontServer = "https://server.wavefront.com";
        static string token = "99999999999999999999";

        static void Main(string[] args)
        {
            startWavefrontReportingViaProxy();
            startWavefrontReportingViaDirectInjestion();

            Console.WriteLine("Hello World!");
        }

        private static void startWavefrontReportingViaProxy()
        {
            IWavefrontSender wavefrontProxyClient = new WavefrontProxyClient.Builder(proxyHost)
                .MetricsPort(metricsPort)
                .Build();

            IMetricsRoot metrics = new MetricsBuilder()
                .Configuration.Configure(options =>
                {
                    options.DefaultContextLabel = "service";
                    options.GlobalTags = new GlobalMetricTags(new Dictionary<string, string>
                    {
                        { "dc", "us-west-2" },
                        { "env", "staging" }
                    });
                })
                .Report.ToWavefront(options =>
                {
                    options.WavefrontSender = wavefrontProxyClient;
                    options.Source = "app-1.company.com";
                })
                .Build();

            CounterOptions evictions = new CounterOptions
            {
                Name = "cache-evictions"
            };
            metrics.Measure.Counter.Increment(evictions);

            var scheduler = new AppMetricsTaskScheduler(TimeSpan.FromSeconds(5), async () =>
            {
                await Task.WhenAll(metrics.ReportRunner.RunAllAsync());
            });
            scheduler.Start();

        }
        private static void startWavefrontReportingViaDirectInjestion()
        {
            IWavefrontSender wavefrontDirectIngestionClient =
                new WavefrontDirectIngestionClient.Builder(wavefrontServer, token).Build();

            IMetricsRoot metrics = new MetricsBuilder()
                .Configuration.Configure(options =>
                {
                    options.DefaultContextLabel = "service";
                    options.GlobalTags = new GlobalMetricTags(new Dictionary<string, string>
                    {
                        { "dc", "us-west-2" },
                        { "env", "staging" }
                    });
                })
                .Report.ToWavefront(options =>
                {
                    options.WavefrontSender = wavefrontDirectIngestionClient;
                    options.Source = "app-1.company.com";
                })
                .Build();

            CounterOptions evictions = new CounterOptions
            {
                Name = "cache-evictions"
            };
            metrics.Measure.Counter.Increment(evictions);

            var scheduler = new AppMetricsTaskScheduler(TimeSpan.FromSeconds(5), async () =>
            {
                await Task.WhenAll(metrics.ReportRunner.RunAllAsync());
            });
            scheduler.Start();
        }
    }
}
