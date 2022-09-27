using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingGauge : WhenTestingCreation<IGauge>
    {
        [When]
        public void WhenGaugeCreated()
        {
            CreatedMetric = MetricFactory.Gauge(DefaultMetricName, DefaultMetricDescription);
        }
    }
}
