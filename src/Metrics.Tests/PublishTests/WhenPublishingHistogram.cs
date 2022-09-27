using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingHistogram : WhenTestingPublish<IHistogram>
    {
        [Given]
        public void WeCreateHistogram()
        {
            Metric = MetricFactory.Histogram(DefaultMetricName, DefaultMetricDescription,
                new double[] { 10, 50, 250 });
        }

        [When]
        public async Task WhenHistogramTracksValues()
        {
            // 4 in <= 10
            Metric.Observe(0);
            Metric.Observe(-10);
            Metric.Observe(9);
            Metric.Observe(10);
            
            // 3 in <= 50
            Metric.Observe(11);
            Metric.Observe(37);
            Metric.Observe(49);
            
            // 5 in <= 250
            Metric.Observe(51);
            Metric.Observe(116);
            Metric.Observe(210);
            Metric.Observe(249);
            Metric.Observe(250);

            // 2 in 250+
            Metric.Observe(251,2);
            Metric.Observe(60000);

            PublishedString = await GetPublishedMetricsString();
        }

        [Then]
        public void CorrectSumExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_sum 61484"));
        }
        
        [Then]
        public void CorrectTotalExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_count 15"));
        }
        
        [Then]
        public void TenBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"10\"} 4"));
        }
        
        [Then]
        public void FiftyBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"50\"} 7"));
        }
        
        [Then]
        public void TwoFiftyBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"250\"} 12"));
        }
        
        [Then]
        public void OverTwoFiftyBucketExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_bucket{le=\"+Inf\"} 15"));
        }

        [Then]
        public void SumHasExpectedValue()
        {
            Assert.That(Metric.Sum, Is.EqualTo(61484));
        }
        
        [Then]
        public void CountHasExpectedValue()
        {
            Assert.That(Metric.Count, Is.EqualTo(15));
        }
    }
}
