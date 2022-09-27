using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingTimerDuplicate : WhenTestingCreateDuplicate<ITimer>
    {
        [When(DoNotRethrowExceptions: true)]
        public void WhenTimersCreated()
        {
            CreatedMetric = MetricFactory.Timer(DefaultMetricName, DefaultMetricDescription);
            DuplicateMetric = MetricFactory.Timer(DefaultMetricName, DefaultMetricDescription);
        }
    }
}
