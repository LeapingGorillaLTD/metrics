using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingGaugeDuplicate : WhenTestingCreateDuplicate<IGauge>
    {
        [When(DoNotRethrowExceptions: true)]
        public void WhenGaugesCreated()
        {
            CreatedMetric = MetricFactory.Gauge(DefaultMetricName, DefaultMetricDescription);
            DuplicateMetric = MetricFactory.Gauge(DefaultMetricName, DefaultMetricDescription);
        }
    }
}
