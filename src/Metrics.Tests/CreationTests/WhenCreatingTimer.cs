using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingTimer : WhenTestingCreation<ITimer>
    {
        [When]
        public void WhenTimerCreated()
        {
            CreatedMetric = MetricFactory.Timer(DefaultMetricName, DefaultMetricDescription);
        }
    }
}
