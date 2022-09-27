using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingCounter : WhenTestingPublish<ICounter>
    {
        [Given]
        public void WeCreateCounter()
        {
            Metric = MetricFactory.Counter(DefaultMetricName, DefaultMetricDescription);
        }

        [When]
        public async Task WhenCounterTracksValues()
        {
            Metric.Increment();
            Metric.IncrementTo(2);
            Metric.Increment();
            PublishedString = await GetPublishedMetricsString();
        }

        [Then]
        public void ExpectedValuesExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric 3"));
        }
        
        [Then]
        public void CounterHasExpectedValue()
        {
            Assert.That(Metric.Value, Is.EqualTo(3));
        }
    }
}
