using LeapingGorilla.Metrics.Prometheus;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit;

namespace LeapingGorilla.Metrics.UnitTests
{
	public abstract class WhenTestingMetrics : WhenTestingTheBehaviourOf
    {
        public static readonly string DefaultMetricName = "Test Metric";
        public static readonly string DefaultMetricDescription = "Test Metric created by unit tests";

        [ItemUnderTest]
        public PrometheusMetricFactory MetricFactory { get; set; }

        [Dependency]
        public string ApplicationName { get; set; }

        protected override void CreateManualDependencies()
        {
            base.CreateManualDependencies();
            ApplicationName = "UnitTests";
        }

        [Given(int.MinValue)]
        public void MetricsAreReset()
        {
            MetricFactory.Reset();
        }
    }
}