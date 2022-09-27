using System;

namespace LeapingGorilla.Metrics
{
	/// <summary>
	/// Represents a Counter. Counters start at zero and only ever increase
	/// in value. Counters reset to zero whenever a process restarts. Counters
	/// are a good choice for tracking the number of times that a thing happened
	/// in the context of the process lifetime. You can use the Prometheus rate()
	/// function against a counter to find the per-second rate at which a counter
	/// increases
	/// </summary>
	public interface ICounter : IMetric
	{
        /// <summary>The current value of the counter</summary>
        double Value { get; }

		/// <summary>
		/// Increase a counter by a given value. Defaults to increasing by 1
		/// </summary>
		/// <param name="amount">The amount that will be added to the counter. Default: 1</param>
		/// <exception cref="InvalidOperationException">Thrown if amount is negative</exception>
		void Increment(double amount = 1.0);

        /// <summary>
        /// Increase the labels attached to a counter by a given value.
        /// </summary>
        /// <param name="amount">The amount that will be added to the counter.</param>
        /// <param name="labels">The labels that we will be incrementing</param>
        /// <exception cref="InvalidOperationException">Thrown if amount is negative</exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <see cref="labels"/> is null or has no items
        /// </exception>
        public void IncrementLabel(double amount, params string[] labels);

        /// <summary>
        /// Increase the labels attached to a counter by 1.
        /// </summary>
        /// <param name="labels">The labels that we will be incrementing</param>
        /// <exception cref="InvalidOperationException">Thrown if amount is negative</exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <see cref="labels"/> is null or has no items
        /// </exception>
        void IncrementLabel(params string[] labels);

		/// <summary>
		/// Increase a counter to a given value.
		/// </summary>
		/// <param name="targetValue">The value that the counter will be set to</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="targetValue"/> is less than the current counter value
        /// </exception>
		void IncrementTo(double targetValue);

        /// <summary>
        /// Increase the labels attached to a counter to a given value.
        /// </summary>
        /// <param name="targetValue">The value that the counter will be set to</param>
        /// <param name="labels">The labels that we will be incrementing</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="targetValue"/> is less than the current counter value
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <see cref="labels"/> is null or has no items
        /// </exception>
        public void IncrementLabelTo(double targetValue, params string[] labels);
    }
}
