using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingCounter : WhenTestingCreation<ICounter>
    {
        [When]
        public void WhenCounterCreated()
        {
            CreatedMetric = MetricFactory.Counter(DefaultMetricName, DefaultMetricDescription);
        }
    }
}