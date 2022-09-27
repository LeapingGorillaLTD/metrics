using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingGauge : WhenTestingPublish<IGauge>
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
            PublishedString = await GetPublishedMetricsString();
        }

        [Then]
        public void ExpectedValuesExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric 1"));
        }
    }
}
