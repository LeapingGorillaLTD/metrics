using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingCounterDuplicate : WhenTestingCreateDuplicate<ICounter>
    {
        [When(DoNotRethrowExceptions: true)]
        public void WhenCountersCreated()
        {
            CreatedMetric = MetricFactory.Counter(DefaultMetricName, DefaultMetricDescription);
            DuplicateMetric = MetricFactory.Counter(DefaultMetricName, DefaultMetricDescription);
        }
    }
}
