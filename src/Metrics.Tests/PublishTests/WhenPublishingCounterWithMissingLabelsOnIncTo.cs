using System;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingCounterWithMissingLabelsOnIncTo : WhenTestingPublish<ICounter>
    {
        private string LabelName;

        [Given]
        public void WeCreateCounter()
        {
            LabelName = "testLabel";
            Metric = MetricFactory.Counter(DefaultMetricName, DefaultMetricDescription, LabelName);
        }

        [When(DoNotRethrowExceptions: true)]
        public void WhenCounterTracksValues()
        {
            Metric.IncrementLabelTo(5);
        }

        [Then]
        public void ExceptionOccurred()
        {
            Assert.That(ThrownException, Is.Not.Null);
        }
        
        [Then]
        public void ExceptionIsExpectedType()
        {
            Assert.That(ThrownException, Is.TypeOf<ArgumentException>());
        }
        
        [Then]
        public void ExceptionReferencesLabels()
        {
            Assert.That(((ArgumentException)ThrownException).ParamName, Is.EqualTo("labelValues"));
        }
    }
}
