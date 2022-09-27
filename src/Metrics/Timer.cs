using System.Diagnostics;

namespace LeapingGorilla.Metrics
{
    /// <inheritdoc cref="ITimer" />
	public class Timer : Metric, ITimer
	{
		private readonly Prometheus.Histogram _histogram;

		public Timer(Prometheus.Histogram histogram, string nameInStorageSystem, string description) 
            :base(nameInStorageSystem, description)
		{
			_histogram = histogram;
		}

        /// <inheritdoc />
		public void Record(long timeTakenInMilliseconds)
		{
			_histogram.Observe(timeTakenInMilliseconds);
		}

        /// <inheritdoc />
		public void Record(Stopwatch stopwatch)
		{
			Record(stopwatch.ElapsedMilliseconds);
		}
	}
}
