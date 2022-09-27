using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingGaugeTrackingInProgress : WhenTestingPublish<IGauge>
    {
        [Given]
        public void WeCreateGauge()
        {
            Metric = MetricFactory.Gauge(DefaultMetricName, DefaultMetricDescription);
        }

        [When]
        public async Task WhenGaugeTracksValues()
        {
            Metric.Increment();
            Metric.Increment();
            Metric.Decrement();
            using var c = Metric.TrackInProgress();
            PublishedString = await GetPublishedMetricsString();
        }

        [Then]
        public void ExpectedValuesExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric 2"));
        }

        [Then]
        public void CountShouldNotIncludeInProgress()
        {
            Assert.That(Metric.Value, Is.EqualTo(1));
        }
    }
}
