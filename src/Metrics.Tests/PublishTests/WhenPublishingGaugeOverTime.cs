using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingGaugeOverTime : WhenTestingPublish<IGauge>
    {
        private string _secondString;

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

            Metric.Increment();
            Metric.Increment();
            Metric.Decrement();
            _secondString = await GetPublishedMetricsString();
        }

        [Then]
        public void ExpectedInitialValuesExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric 1"));
        }
        
        [Then]
        public void ExpectedSecondValuesExported()
        {
            Assert.That(_secondString, Contains.Substring("unittests_test_metric 2"));
        }
    }
}
