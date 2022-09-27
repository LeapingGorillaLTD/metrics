using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingSummary : WhenTestingCreation<ISummary>
    {
        [When]
        public void WhenSummaryCreated()
        {
            CreatedMetric = MetricFactory.Summary(DefaultMetricName, DefaultMetricDescription);
        }
    }
}
