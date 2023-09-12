using System;
using System.Linq;
using LeapingGorilla.Metrics.Exceptions;
using Prometheus;

namespace LeapingGorilla.Metrics.Prometheus
{
	public class PrometheusMetricFactory : MetricFactory
	{
        private global::Prometheus.MetricFactory _factory;

        /// <summary>The registry used by the underlying Prometheus implementation. Used for unit testing.</summary>
        internal CollectorRegistry Registry;

        /// <summary>The default quantiles that we will use for summaries where no other is given</summary>
        private static readonly SummaryObjective[] DefaultQuantiles = 
        {
            new(0.5, 0.01),     //p50   (Error allowance is p49-51)
            new(0.75, 0.01),    //p50   (Error allowance is p74-76)
            new(0.9, 0.01),     //p90   (Error allowance is p89-91)
            new(0.95, 0.01),    //p95   (Error allowance is p94-96)
            new(0.99, 0.005),   //p99   (Error allowance is p98.5-99.5)
            new(0.999, 0.001),  //p99.9 (Error allowance is p99.8-p100)
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
		public PrometheusMetricFactory(string applicationName) : base(applicationName)
        {
            Registry = global::Prometheus.Metrics.DefaultRegistry;
            _factory = global::Prometheus.Metrics.WithCustomRegistry(Registry);

            TotalMetrics = new Gauge(_factory.CreateGauge("metrics_count", "Tracks Total number of metrics currently used by this application"), 
                "metrics_count", "Tracks Total number of metrics currently used by this application");
            TotalMetrics.Set(Metrics.Count);
        }

        /// <inheritdoc />
		public override ICounter Counter(string name, string description, params string [] labelNames)
        {
            var metricName = CreateMetricName(name);
            var ctr = Metrics.GetOrAdd(metricName, n => new Counter(_factory.CreateCounter(n, description, labelNames), n, description));
            TotalMetrics.Set(Metrics.Count);
            return ctr as ICounter ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, ctr);
        }

        /// <inheritdoc />
		public override IGauge Gauge(string name, string description)
		{
            var metricName = CreateMetricName(name);
            var gauge = Metrics.GetOrAdd(metricName, n => new Gauge(_factory.CreateGauge(n, description), n, description));
            TotalMetrics.Set(Metrics.Count);
            return gauge as IGauge ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, gauge);
		}

        /// <inheritdoc />
		public override ITimer Timer(string name, string description)
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
            TotalMetrics.Set(Metrics.Count);
            return hg as ITimer ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, hg);
        }
        
        /// <inheritdoc />
        public override ISummary Summary(
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
            TotalMetrics.Set(Metrics.Count);
            return summary as ISummary ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, summary);
        }
        
        /// <inheritdoc />
        public override IHistogram Histogram(string name,  string description, double[] buckets)
        {
            var metricName = CreateMetricName(name);
            var hist = Metrics.GetOrAdd(metricName, n =>
            {
                var config = new HistogramConfiguration
                {
                    Buckets = buckets
                };
                var h = _factory.CreateHistogram(n, description, config);
                return new Histogram(h, n, description);
            });
            
            TotalMetrics.Set(Metrics.Count);
            return hist as IHistogram ?? throw new MetricAlreadyExistsForDifferentTypeException(name, metricName, hist);
        }

        /// <summary>
        /// Reset the factory removing all existing metrics. Used for unit testing.
        /// </summary>
        internal void Reset()
        {
            Metrics.Clear();
            Metrics.TryAdd(TotalMetrics.NameInStorageSystem, TotalMetrics);
            TotalMetrics.Set(Metrics.Count);
            Registry = global::Prometheus.Metrics.NewCustomRegistry();
            _factory = global::Prometheus.Metrics.WithCustomRegistry(Registry);
        }
	}
}