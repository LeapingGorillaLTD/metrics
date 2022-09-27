using System.Diagnostics;
using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    /// <summary>Note: Timer is underpinned by a Histogram so we test it in the same way</summary>
    public class WhenPublishingTimerUsingStopwatch : WhenTestingPublish<ITimer>
    {
        private Stopwatch _stopwatch;

        [Given]
        public void WeCreateTimer()
        {
            Metric = MetricFactory.Timer(DefaultMetricName, DefaultMetricDescription);
        }

        [Given]
        public void WeHaveStopwatch()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        [When]
        public async Task WhenTimerTracksValues()
        {
            await Task.Delay(1, TokenSource.Token);
            
            Metric.Record(_stopwatch);

            PublishedString = await GetPublishedMetricsString();
        }
        
        [Then]
        public void CorrectTotalExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_count 1"));
        }
        
        [Then]
        public void Over10mBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"+Inf\"} 1"));
        }
    }
}
