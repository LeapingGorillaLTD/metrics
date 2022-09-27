using System;
using Prometheus;

namespace LeapingGorilla.Metrics
{
    /// <inheritdoc cref="IGauge" />
	public class Gauge : Metric, IGauge
	{
		private readonly Prometheus.Gauge _gauge;

        public double Value => _gauge.Value;

		public Gauge(Prometheus.Gauge gauge, string nameInStorageSystem, string description) 
            :base(nameInStorageSystem, description)
		{
			_gauge = gauge;
		}

        /// <inheritdoc />
        public void Increment(double amount = 1)
		{
			_gauge.Inc(amount);
        }

        /// <inheritdoc />
		public void Decrement(double amount = 1)
		{
			_gauge.Dec(amount);
		}

        /// <inheritdoc />
		public void Set(double value)
		{
			_gauge.Set(value);
		}

        /// <inheritdoc />
        public IDisposable TrackInProgress()
        {
            return _gauge.TrackInProgress();
        }
	}
}
