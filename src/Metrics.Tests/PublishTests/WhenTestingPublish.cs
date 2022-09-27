using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public abstract class WhenTestingPublish<T> : WhenTestingMetrics where T : IMetric
    {
        protected T Metric;

        protected string PublishedString;

        protected readonly CancellationTokenSource TokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        
        protected async Task<string> GetPublishedMetricsString()
        {
            await using var ms = new MemoryStream();
            await MetricFactory.Registry.CollectAndExportAsTextAsync(ms, TokenSource.Token);
            ms.Seek(0, SeekOrigin.Begin);

            using var sr = new StreamReader(ms);
            return await sr.ReadToEndAsync();
        }
    }
}
