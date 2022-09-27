using System;
using System.Diagnostics.CodeAnalysis;

namespace LeapingGorilla.Metrics.Exceptions
{
    /// <summary>
    /// Exception thrown when you try to create a metric with the same name as an
    /// existing metric which has a different type.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MetricAlreadyExistsForDifferentTypeException : MetricsException
    {
        private const string DefaultMessage = "A metric already exists with the given name but for a different metric type";

        /// <summary>Name of the metric as you passed it to the MetricFactory</summary>
        public string Name { get; }

        /// <summary>
        /// Name of the metric in the metric system (i.e. in Prometheus)
        /// </summary>
        public string MetricSystemName { get; }

        /// <summary>
        /// The existing metric in the system which conflicted with this name and type
        /// </summary>
        public IMetric ExistingMetric { get; }

        public MetricAlreadyExistsForDifferentTypeException(
            string name, 
            string metricSystemName, 
            IMetric existingMetric, 
            Exception? innerException = null) : base(DefaultMessage, innerException)
        {
            Name = name;
            MetricSystemName = metricSystemName;
            ExistingMetric = existingMetric;
        }
    }
}
