using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingHistogramDuplicate : WhenTestingCreateDuplicate<IHistogram>
    {
        [When(DoNotRethrowExceptions: true)]
        public void WhenHistogramsCreated()
        {
            CreatedMetric = MetricFactory.Histogram(DefaultMetricName, DefaultMetricDescription, new double[] { 10, 50, 100 });
            DuplicateMetric = MetricFactory.Histogram(DefaultMetricName, DefaultMetricDescription, new double[] { 10, 50, 100 });
        }
    }
}
