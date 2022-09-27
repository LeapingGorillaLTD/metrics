using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public abstract class WhenTestingCreation<T> : WhenTestingMetrics where T : IMetric
    {
        protected T CreatedMetric;

        [Then]
        public void MetricIsCreated()
        {
            Assert.That(CreatedMetric, Is.Not.Null);
        }
        
        [Then]
        public void MetricIsExpectedType()
        {
            Assert.That(CreatedMetric, Is.InstanceOf<T>());
        }

        [Then]
        public void MetricHasExpectedName()
        {
            // {application}_{environment}_{metric name}
            Assert.That(CreatedMetric.NameInStorageSystem, Is.EqualTo("unittests_test_metric"));
        }

        [Then]
        public void MetricHasExpectedDescription()
        {
            Assert.That(CreatedMetric.Description, Is.EqualTo(DefaultMetricDescription));
        }
    }
}
