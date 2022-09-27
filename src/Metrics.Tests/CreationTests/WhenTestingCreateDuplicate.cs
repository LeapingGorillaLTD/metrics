using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.CreationTests
{
    public abstract class WhenTestingCreateDuplicate<T> : WhenTestingCreation<T> where T: IMetric
    {
        protected T DuplicateMetric;

        [Then]
        public void NoExceptionThrown()
        {
            Assert.That(ThrownException, Is.Null);
        }

        [Then]
        public void SameMetricReturnedTwice()
        {
            Assert.That(CreatedMetric, Is.EqualTo(DuplicateMetric));
        }
    }
}
