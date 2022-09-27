using LeapingGorilla.Testing.Core.Attributes;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public class WhenCreatingHistogram : WhenTestingCreation<IHistogram>
    {
        [When]
        public void WhenHistogramCreated()
        {
            CreatedMetric = MetricFactory.Histogram(DefaultMetricName, DefaultMetricDescription, new double[] { 20, 100, 500, 1000 });
        }
    }
}
