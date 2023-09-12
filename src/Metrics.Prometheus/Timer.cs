using System.Diagnostics;

namespace LeapingGorilla.Metrics.Prometheus
{
    /// <inheritdoc cref="ITimer" />
	public class Timer : Metric, ITimer
	{
		private readonly global::Prometheus.Histogram _histogram;

		public Timer(global::Prometheus.Histogram histogram, string nameInStorageSystem, string description) 
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
