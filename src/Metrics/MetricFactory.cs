using System;
using System.Collections.Concurrent;
using System.Linq;
using LeapingGorilla.Metrics.Exceptions;
using Prometheus;

namespace LeapingGorilla.Metrics
{
	public class MetricFactory : IMetricFactory
	{
		/// <summary>Name of the application we are creating metrics for</summary>
		private readonly string _applicationName;

        /// <summary>Number of milliseconds in a second</summary>
        private const double Seconds = 1000;

        /// <summary>Number of milliseconds in a minute</summary>
        private const double Minutes = 1000 * 60;

        /// <summary>The default period of time that summaries will calculate their objectives over</summary>
        private static readonly TimeSpan DefaultSummaryTimeSpan = new TimeSpan(0, 0, 10, 0);

        /// <summary>
        /// Stores all metrics that have already been instantiated - we want to keep one instance per metric.
        /// </summary>
        private readonly ConcurrentDictionary<string, IMetric> Metrics = new ConcurrentDictionary<string, IMetric>();

        /// <summary>Tracks the number of metrics tracked by the current application</summary>
        private readonly IGauge _totalMetrics;

        private Prometheus.MetricFactory _factory;

        /// <summary>The registry used by the underlying Prometheus implementation. Used for unit testing.</summary>
        internal CollectorRegistry Registry;

        /// <summary>The default quantiles that we will use for summaries where no other is given</summary>
        private static readonly SummaryObjective[] DefaultQuantiles = 
        {
            new SummaryObjective(0.5, 0.01),     //p50   (Error allowance is p49-51)
            new SummaryObjective(0.75, 0.01),    //p50   (Error allowance is p74-76)
            new SummaryObjective(0.9, 0.01),     //p90   (Error allowance is p89-91)
            new SummaryObjective(0.95, 0.01),    //p95   (Error allowance is p94-96)
            new SummaryObjective(0.99, 0.005),   //p99   (Error allowance is p98.5-99.5)
            new SummaryObjective(0.999, 0.001),  //p99.9 (Error allowance is p99.8-p100)
        };

		/// <summary>
		/// Create a new MetricFactory for a given application.
		/// </summary>
		/// <param name="applicationName">
		/// Name of the application we are creating metrics for. This will be used to
		/// prefix the name of any other metrics that are created. Spaces will be removed.
		/// The name will be converted to lower case. Underscores are considered special
		/// characters in Prometheus so use them at your peril
		/// </param>
		public MetricFactory(string applicationName)
        {
            _applicationName = applicationName.ToLowerInvariant().Replace(" ", String.Empty).Trim('_');
            Registry = Prometheus.Metrics.DefaultRegistry;
            _factory = Prometheus.Metrics.WithCustomRegistry(Registry);

            _totalMetrics = new Gauge(_factory.CreateGauge("metrics_count", "Tracks Total number of metrics currently used by this application"), 
                "metrics_count", "Tracks Total number of metrics currently used by this application");
            _totalMetrics.Set(Metrics.Count);
        }

        /// <inheritdoc />
		public ICounter Counter(string name, string description, params string [] labelNames)
        {
            var metricName = CreateMetricName(name);
            var ctr = Metrics.GetOrAdd(metricName, n => new Counter(_factory.CreateCounter(n, description, labelNames), n, description));
            _totalMetrics.Set(Metrics.Count);
            return ctr as ICounter ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, ctr);
        }

        /// <inheritdoc />
		public IGauge Gauge(string name, string description)
		{
            var metricName = CreateMetricName(name);
            var gauge = Metrics.GetOrAdd(metricName, n => new Gauge(_factory.CreateGauge(n, description), n, description));
            _totalMetrics.Set(Metrics.Count);
            return gauge as IGauge ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, gauge);
		}

        /// <inheritdoc />
		public ITimer Timer(string name, string description)
		{
            var metricName = CreateMetricName(name);
            var hg = Metrics.GetOrAdd(metricName, n =>
            {
                var config = new HistogramConfiguration {Buckets = new []
                {
                    10, // ms
                    25, // ms
                    50, // ms
                    100,// ms 
                    250,// ms
                    500,// ms
                    1   * Seconds, 
                    2.5 * Seconds, 
                    5   * Seconds, 
                    10  * Seconds, 
                    15  * Seconds, 
                    20  * Seconds, 
                    25  * Seconds, 
                    30  * Seconds, 
                    60  * Seconds, 
                    3   * Minutes, 
                    5   * Minutes, 
                    10  * Minutes
                }};
                var hist = _factory.CreateHistogram(n, description, config);
                return new Timer(hist, n, description);
            });
            _totalMetrics.Set(Metrics.Count);
            return hg as ITimer ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, hg);
        }
        
        /// <inheritdoc />
        public ISummary Summary(
            string name, 
            string description, 
            TimeSpan? overTimePeriod = null,
            SummaryObjective[]? objectives = null
        )
        {
            var objectivePairs = objectives ?? DefaultQuantiles;

            var metricName = CreateMetricName(name);
            var summary = Metrics.GetOrAdd(metricName, n =>
            {
                var config = new SummaryConfiguration
                {
                    Objectives = objectivePairs.Select(o => o.ToQuantileEpsilonPair()).ToArray(),
                    MaxAge = overTimePeriod ?? DefaultSummaryTimeSpan,
                };

                var s = _factory.CreateSummary(n, description, config);
                return new Summary(s, n, description);
            });
            _totalMetrics.Set(Metrics.Count);
            return summary as ISummary ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, summary);
        }
        
        /// <inheritdoc />
        public IHistogram Histogram(string name,  string description, double[] buckets)
        {
            var metricName = CreateMetricName(name);
            var hist = Metrics.GetOrAdd(metricName, n =>
            {
                var config = new HistogramConfiguration
                {
                    Buckets = buckets
                };
                var h = _factory.CreateHistogram(metricName, description, config);
                return new Histogram(h, n, description);
            });
            
            _totalMetrics.Set(Metrics.Count);
            return hist as IHistogram ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, hist);
        }

		/// <summary>
		/// Creates the name of a metric in the style <code>{application}_{name}</code>.
		/// Metric names are always in lowercase, spaces in the name are replaced with underscores. 
		/// </summary>
		/// <param name="name">Name for the metric being created</param>
		/// <returns>Name for a metric</returns>
		private string CreateMetricName(string name)
		{
			return $"{_applicationName}_{name.ToLowerInvariant().Replace(" ", "_").Trim('_')}";
		}

        /// <summary>
        /// Reset the factory removing all existing metrics. Used for unit testing.
        /// </summary>
        internal void Reset()
        {
            Metrics.Clear();
            Metrics.TryAdd(_totalMetrics.NameInStorageSystem, _totalMetrics);
            _totalMetrics.Set(Metrics.Count);
            Registry = Prometheus.Metrics.NewCustomRegistry();
            _factory = Prometheus.Metrics.WithCustomRegistry(Registry);
        }
	}
}
