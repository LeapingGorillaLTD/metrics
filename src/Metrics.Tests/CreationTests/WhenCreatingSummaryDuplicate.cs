using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingSummaryDuplicate : WhenTestingCreateDuplicate<ISummary>
    {
        [When(DoNotRethrowExceptions: true)]
        public void WhenHistogramsCreated()
        {
            CreatedMetric = MetricFactory.Summary(DefaultMetricName, DefaultMetricDescription);
            DuplicateMetric = MetricFactory.Summary(DefaultMetricName, DefaultMetricDescription);
        }
    }
}
