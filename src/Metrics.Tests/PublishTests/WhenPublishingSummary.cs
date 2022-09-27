using System.Threading.Tasks;
using LeapingGorilla.Testing.Core.Attributes;
using LeapingGorilla.Testing.NUnit.Attributes;
using NUnit.Framework;

namespace LeapingGorilla.Metrics.UnitTests.PublishTests
{
    public class WhenPublishingSummary : WhenTestingPublish<ISummary>
    {
        [Given]
        public void WeCreateSummary()
        {
            Metric = MetricFactory.Summary(DefaultMetricName, DefaultMetricDescription);
        }

        [When]
        public async Task WhenSummaryTracksValues()
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
            Metric.Observe(251);
            Metric.Observe(60000);

            PublishedString = await GetPublishedMetricsString();
        }

        [Then]
        public void CorrectSumExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_sum 61233"));
        }
        
        [Then]
        public void CorrectTotalExported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric_count 14"));
        }
        
        [Then]
        public void P50Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{quantile=\"0.5\"} 49"));
        }
        
        [Then]
        public void P75Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{quantile=\"0.75\"} 210"));
        }
        
        [Then]
        public void P90Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{quantile=\"0.9\"} 250"));
        }
        
        [Then]
        public void P95Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{quantile=\"0.95\"} 251"));
        }
        
        [Then]
        public void P99Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{quantile=\"0.99\"} 251"));
        }
        
        [Then]
        public void P99_9Exported()
        {
            Assert.That(PublishedString, Contains.Substring("unittests_test_metric{quantile=\"0.99\"} 251"));
        }
    }
}
