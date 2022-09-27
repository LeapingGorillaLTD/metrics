using System.Diagnostics;

namespace LeapingGorilla.Metrics
{
	/// <summary>
	/// Uses an underlying histogram to track time taken for a given operation. The histogram is bucketed
	/// in the following millisecond steps:
    ///
    /// under 50, 100, 250, 500, 1000, 2500, 5000, 10000, 15000, 20000, 25000, 30000, 60000 (1 min),
    /// 150000 (2.5 mins), 300000 (5 mins), 600000 (10 mins) and over 10 mins
    ///
    /// Timers are good for timing request durations or for timing method calls. Timers always
    /// report in milliseconds.
	/// </summary>
	public interface ITimer : IMetric
	{
		/// <summary>
		/// Record the amount of time that was taken for this metric
		/// </summary>
		/// <param name="timeTakenInMilliseconds">The time taken in milliseconds (1 second = 1000 milliseconds)</param>
		void Record(long timeTakenInMilliseconds);

		/// <summary>
		/// Record the amount of time that was taken for this metric from the passed <see cref="Stopwatch"/>
		/// </summary>
		/// <param name="stopwatch">Stopwatch we will record the Elapsed Milliseconds from</param>
		void Record(Stopwatch stopwatch);
	}
}

