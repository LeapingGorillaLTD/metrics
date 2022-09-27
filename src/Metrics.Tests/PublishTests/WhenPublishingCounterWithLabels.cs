using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingCounterWithLabels : WhenTestingPublish<ICounter>
    {
        private string LabelName;

        [Given]
        public void WeCreateCounter()
        {
            LabelName = "testlabel";
            Metric = MetricFactory.Counter(DefaultMetricName, DefaultMetricDescription, LabelName);
        }

        [When]
        public async Task WhenCounterTracksValues()
        {
            Metric.IncrementLabel("label1");
            Metric.IncrementLabel("label2");
            Metric.IncrementLabel("label3");
            Metric.IncrementLabelTo(5, "label2");
            Metric.IncrementLabelTo(3, "label1");
            Metric.Increment();
            
            PublishedString = await GetPublishedMetricsString();
        }

        [Then]
        public void Label1Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{testlabel=\"label1\"} 3"));
        }
        
        [Then]
        public void Label2Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{testlabel=\"label2\"} 5"));
        }
        
        [Then]
        public void Label3Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{testlabel=\"label3\"} 1"));
        }
        
        [Then]
        public void NoLabelExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric 1"));
        }
        
        [Then]
        public void CounterHasExpectedValue()
        {
            Assert.That(Metric.Value, Is.EqualTo(1));
        }
    }
}
