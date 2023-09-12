using System;
using System.Collections.Concurrent;

namespace LeapingGorilla.Metrics
{
	public abstract class MetricFactory : IMetricFactory
	{
		/// <summary>Name of the application we are creating metrics for</summary>
		protected readonly string _applicationName;

        /// <summary>Number of milliseconds in a second</summary>
        protected const double Seconds = 1000;

        /// <summary>Number of milliseconds in a minute</summary>
        protected const double Minutes = 1000 * 60;

        /// <summary>The default period of time that summaries will calculate their objectives over</summary>
        protected static readonly TimeSpan DefaultSummaryTimeSpan = new(0, 0, 10, 0);

        /// <summary>
        /// Stores all metrics that have already been instantiated - we want to keep one instance per metric.
        /// </summary>
        protected readonly ConcurrentDictionary<string, IMetric> Metrics = new();

        /// <summary>Tracks the number of metrics tracked by the current application</summary>
        protected IGauge TotalMetrics { get; set; } = default!;

		/// <summary>
		/// Create a new MetricFactory for a given application.
		/// </summary>
		/// <param name="applicationName">
		/// Name of the application we are creating metrics for. This will be used to
		/// prefix the name of any other metrics that are created. Spaces will be removed.
		/// The name will be converted to lower case. Underscores are considered special
		/// characters in Prometheus so use them at your peril
		/// </param>
		protected MetricFactory(string applicationName)
        {
            _applicationName = applicationName.ToLowerInvariant().Replace(" ", String.Empty).Trim('_');
        }

		/// <inheritdoc />
		public abstract ICounter Counter(string name, string description, params string[] labelNames);

		/// <inheritdoc />
		public abstract IGauge Gauge(string name, string description);

		/// <inheritdoc />
		public abstract ITimer Timer(string name, string description);

		/// <inheritdoc />
		public abstract ISummary Summary(
			string name,
			string description,
			TimeSpan? overTimePeriod = null,
			SummaryObjective[]? objectives = null
		);

		/// <inheritdoc />
		public abstract IHistogram Histogram(string name, string description, double[] buckets);

		/// <summary>
		/// Creates the name of a metric in the style <code>{application}_{name}</code>.
		/// Metric names are always in lowercase, spaces in the name are replaced with underscores. 
		/// </summary>
		/// <param name="name">Name for the metric being created</param>
		/// <returns>Name for a metric</returns>
		protected virtual string CreateMetricName(string name)
		{
			return $"{_applicationName}_{name.ToLowerInvariant().Replace(" ", "_").Trim('_')}";
		}
	}
}