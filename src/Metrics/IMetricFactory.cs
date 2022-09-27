using System;

namespace LeapingGorilla.Metrics
{
	/// <summary>
	/// Creates metrics ready for reporting to the underlying metric system
	/// </summary>
	public interface IMetricFactory
	{
		/// <summary>
		/// Creates a <see cref="ICounter"/>. Counters start at zero and only
		/// ever increase. Counters will reset to zero whenever the process is
		/// restarted.
		/// </summary>
		/// <param name="name">
		/// Name of the metric in the underlying monitoring system. This should be in the format of:
		///
		///		{Metric_Name}_{Unit}
		/// 
		/// all in lower case i.e.
		///
		///		rules_invoked_total
		/// 
		/// see <see href="https://prometheus.io/docs/practices/naming/">the Prometheus docs</see> for
		/// more information. The metric factory will prefix the application name.
		/// </param>
		/// <param name="description">
		/// A description that will be added to your metric in the metric monitoring system. This
		/// should allow somebody looking at the metric in the database understand the context of the
		/// metric i.e.
		///
		/// "Tracks the number of times that a CrossingGuard rule has been invoked" 
		/// </param>
		/// <param name="labelNames">The names of the labels you will gather for this counter. i.e. "HttpMethod",
		/// "Endpoint" or "AccountNumber". This should be the overarching name for the label.</param>
		/// <returns><see cref="ICounter"/> that can be used to track an incrementing number</returns>
		ICounter Counter(string name, string description, params string [] labelNames);

		/// <summary>
		/// Creates a <see cref="IGauge"/>. Gauges can track an arbitrary number
		/// that may increase or decrease. Gauges may track negative numbers. Gauges
		/// are useful to track values in a sliding scale i.e. the number of items
		/// waiting in a queue, current CPU load or number of items in flight.
		/// </summary>
		/// <param name="name">
		/// Name of the metric in the underlying monitoring system. This should be in the format of:
		///
		///		{Metric_Name}_{Unit}
		/// 
		/// all in lower case i.e.
		///
		///		messages_pending_queue_total
		/// 
		/// see <see href="https://prometheus.io/docs/practices/naming/">the Prometheus docs</see> for
		/// more information. The metric factory will prefix the application name.
		/// </param>
		/// <param name="description">
		/// A description that will be added to your metric in the metric monitoring system. This
		/// should allow somebody looking at the metric in the database understand the context of the
		/// metric i.e.
		///
		/// "Tracks the number of queued items waiting to be run against the rules system" 
		/// </param>
		/// <returns><see cref="IGauge"/> that can be used to track an arbitrarily changing number</returns>
		IGauge Gauge(string name, string description);

		/// <summary>
		/// Creates a <see cref="ITimer"/> which uses an underlying histogram to track time taken
		/// for a given operation. The histogram is bucketed in the following millisecond steps:
		///
		/// under 50, 100, 250, 500, 1000, 2500, 5000, 10000, 15000, 20000, 25000, 30000, 60000 (1 min),
		/// 150000 (2.5 mins), 300000 (5 mins), 600000 (10 mins) and over 10 mins
		///
		/// Timers are good for timing request durations or for timing method calls. Timers always
		/// report in milliseconds.
		/// </summary>
		/// <param name="name">
		/// Name of the metric in the underlying monitoring system. This should be in the format of:
		///
		///		{Metric_Name}_{Unit}
		/// 
		/// all in lower case i.e.
		///
		///		basic_rule_time_taken_ms
		/// 
		/// see <see href="https://prometheus.io/docs/practices/naming/">the Prometheus docs</see> for
		/// more information. The metric factory will prefix the application name.
		/// </param>
		/// <param name="description">
		/// A description that will be added to your metric in the metric monitoring system. This
		/// should allow somebody looking at the metric in the database understand the context of the
		/// metric i.e.
		///
		/// "Tracks the average time taken for the basic rule to be run against a message" 
		/// </param>
		/// <returns>
		/// <see cref="ITimer"/> that can be used to track the time taken for a given action. P values
		/// can be exposed using the histogram_quantile() function on the server
		/// </returns>
		ITimer Timer(string name, string description);


        /// <summary>
        /// Creates a <see cref="ISummary"/> which tracks the total and sum of values observed over
        /// a sliding time window. Summaries are calculated on the client not the server. If you want
        /// to aggregate summaries from many sources to calculate percentiles you should use a Histogram
        /// which can aggregate on the server.
        ///
        /// Use a summary when you don't know the range of values that you are tracking. Otherwise if
        /// you know the range of values you are measuring then prefer using a <see cref="IHistogram"/>.
        /// </summary>
        /// <param name="name">
        /// Name of the metric in the underlying monitoring system. This should be in the format of:
        ///
        ///		{Metric_Name}_{Unit}
        /// 
        /// all in lower case i.e.
        ///
        ///		basic_rule_time_taken_ms
        /// 
        /// see <see href="https://prometheus.io/docs/practices/naming/">the Prometheus docs</see> for
        /// more information. The metric factory will prefix the application name.
        /// </param>
        /// <param name="description">
        /// A description that will be added to your metric in the metric monitoring system. This
        /// should allow somebody looking at the metric in the database understand the context of the
        /// metric i.e.
        ///
        /// "Tracks the average time taken for the basic rule to be run against a message" 
        /// </param>
        /// <param name="overTimePeriod">
        /// The sliding time window that P values will be calculated over. Default: 10 minutes
        /// </param>
        /// <param name="objectives">
        /// Collection of P-Values which will be tracked along with their permitted error range. Defaults
        /// to collecting p50, p75, p90, p95, p99 and p99.9 with 1 to 0.1% error ranges
        /// </param>
        /// <returns>
        /// <see cref="ISummary"/> that can be used to track percentile values over time
        /// </returns>
        ISummary Summary(string name, string description, TimeSpan? overTimePeriod = null, SummaryObjective[]? objectives = null);


        /// <summary>
        /// Samples observations and counts them in configurable buckets. Also provides
        /// a Sum and Count of all observed values. Quantiles (i.e. P values) can be
        /// calculated on the server by using the histogram_quantile() function.
        ///
        /// If you don't know the range of values that will be measured and therefore can't
        /// define your buckets then use a <see cref="ISummary"/>.
        ///
        /// See https://prometheus.io/docs/concepts/metric_types/#histogram for more
        /// </summary>
        /// <param name="name">
        /// Name of the metric in the underlying monitoring system. This should be in the format of:
        ///
        ///		{Metric_Name}_{Unit}
        /// 
        /// all in lower case i.e.
        ///
        ///		basic_rule_time_taken_ms
        /// 
        /// see <see href="https://prometheus.io/docs/practices/naming/">the Prometheus docs</see> for
        /// more information. The metric factory will prefix the application name.
        /// </param>
        /// <param name="description">
        /// A description that will be added to your metric in the metric monitoring system. This
        /// should allow somebody looking at the metric in the database understand the context of the
        /// metric i.e.
        ///
        /// "Tracks the average time taken for the basic rule to be run against a message" 
        /// </param>
        /// <param name="buckets">
        /// Ascending series of "max" bucket values that will be used to create a range. i.e.
        /// passing: "50, 100, 500, 1000" would create buckets of: "less than or equal to 50",
        /// "over 50 but less than or equal to 100", "over 10 but less than or equal to 500",
        /// "over 500 but less than or equal to 1000" and "greater than 1000"
        /// </param>
        /// <returns>
        /// <see cref="IHistogram"/> that can be used to track values across bucket ranges
        /// </returns>
        IHistogram Histogram(string name, string description, double[] buckets);
    }
}
