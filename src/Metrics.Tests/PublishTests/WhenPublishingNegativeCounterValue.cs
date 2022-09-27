using System;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingNegativeCounterValue : WhenTestingPublish<ICounter>
    {
        [Given]
        public void WeCreateCounter()
        {
            Metric = MetricFactory.Counter(DefaultMetricName, DefaultMetricDescription);
        }

        [When(DoNotRethrowExceptions: true)]
        public void WhenCounterTracksValues()
        {
            Metric.Increment(-1);
        }

        [Then]
        public void ExceptionThrown()
        {
            Assert.That(ThrownException, Is.Not.Null);
        }
        
        [Then]
        public void ExceptionIsExpectedType()
        {
            Assert.That(ThrownException, Is.TypeOf<InvalidOperationException>());
        }
    }
}
