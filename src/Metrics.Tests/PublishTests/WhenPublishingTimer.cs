using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    /// <summary>Note: Timer is underpinned by a Histogram so we test it in the same way</summary>
    public class WhenPublishingTimer : WhenTestingPublish<ITimer>
    {
        [Given]
        public void WeCreateTimer()
        {
            Metric = MetricFactory.Timer(DefaultMetricName, DefaultMetricDescription);
        }

        [When]
        public async Task WhenTimerTracksValues()
        {
            Metric.Record(0);
            Metric.Record(9);
            Metric.Record(10);
            Metric.Record(11);
            Metric.Record(37);
            Metric.Record(49);
            Metric.Record(51);
            Metric.Record(116);
            Metric.Record(210);
            Metric.Record(249);
            Metric.Record(250);
            Metric.Record(251);
            Metric.Record(500);
            Metric.Record(10000);
            Metric.Record(10001);
            Metric.Record(25000);
            Metric.Record(60000);

            PublishedString = await GetPublishedMetricsString();
        }

        [Then]
        public void CorrectSumExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_sum 106744"));
        }
        
        [Then]
        public void CorrectTotalExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_count 17"));
        }
        
        [Then]
        public void _50msBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"50\"} 6"));
        }
        
        [Then]
        public void _100msBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"100\"} 7"));
        }
        
        [Then]
        public void _250msBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"250\"} 11"));
        }
        
        [Then]
        public void _500msBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"500\"} 13"));
        }
        
        [Then]
        public void _1sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"1000\"} 13"));
        }
        
        [Then]
        public void _2_5sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"2500\"} 13"));
        }
        
        [Then]
        public void _5sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"5000\"} 13"));
        }
        
        [Then]
        public void _10sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"10000\"} 14"));
        }
        
        [Then]
        public void _15sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"15000\"} 15"));
        }
        
        [Then]
        public void _20sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"20000\"} 15"));
        }
        
        [Then]
        public void _25sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"25000\"} 16"));
        }
        
        [Then]
        public void _30sBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"30000\"} 16"));
        }
        
        [Then]
        public void _1mBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"60000\"} 17"));
        }
        
        [Then]
        public void _3mBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"180000\"} 17"));
        }
        
        [Then]
        public void _5mBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"300000\"} 17"));
        }
        
        [Then]
        public void _10mBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"600000\"} 17"));
        }
        
        [Then]
        public void Over10mBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"+Inf\"} 17"));
        }
    }
}
