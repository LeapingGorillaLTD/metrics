using System;

namespace LeapingGorilla.Metrics
{
	/// <summary>
	/// Gauges represent any numeric value and can change arbitrarily. Gauges are a
	/// good choice for tracking the number of items in a queue
	/// </summary>
	public interface IGauge : IMetric
	{
        /// <summary>The current value of the Gauge</summary>
        double Value { get; }

		/// <summary>
		/// Increase the gauge by a given value. Defaults to increasing by 1
		/// </summary>
		/// <param name="amount">The amount that will be added to the gauge. Default: 1</param>
		void Increment(double amount = 1.0);

		/// <summary>
		/// Decrease a gauge by a given value.
		/// </summary>
		/// <param name="amount">The amount that will be subtracted from the gauge. Default: 1</param>
		void Decrement(double amount = 1.0);

		/// <summary>
		/// Set the gauge to an explicit value
		/// </summary>
		/// <param name="value">The explicit value that the gauge will be set to</param>
		void Set(double value);

        /// <summary>
        /// Increments the current gauge when called, automatically decrements
        /// the gauge when disposed. Use this to track the current number of
        /// concurrent operations that are taking place.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> object that you can wrap in a using</returns>
        /// <example>
        /// <code>
        ///     private static readonly myGauge = myMetricFactory.Gauge("JobsBeingProcessed",
        ///         "Number of jobs currently being processed");
        ///
        ///     // ...
        ///
        ///     using (myGauge.TrackInProgress())
        ///     {
        ///         // Code here that processes a job
        ///     }
        /// </code>
        /// </example>
        IDisposable TrackInProgress();
    }
}
