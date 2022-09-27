using System;
using System.Linq;

namespace LeapingGorilla.Metrics
{
    /// <inheritdoc cref="ICounter" />
    public class Counter : Metric, ICounter
	{
		private readonly Prometheus.Counter _ctr;

        public double Value => _ctr.Value;

		public Counter(Prometheus.Counter ctr, string nameInStorageSystem, string description) 
            :base(nameInStorageSystem, description)
		{
			_ctr = ctr;
		}

        /// <inheritdoc />
        public void Increment(double amount = 1.0)
		{
            if (amount < 0)
			{
				throw new InvalidOperationException("Counters may only be incremented. If you need to reduce the number you are recording try a Gauge instead");
			}
            
			_ctr.Inc(amount);
		}
		
        /// <inheritdoc />
		public void IncrementTo(double targetValue)
		{
            lock (_ctr)
            {
                if (targetValue < _ctr.Value)
                {
                    throw new InvalidOperationException("You may not increment a counter to a value lower than it's current value. If you need to reduce the number you are recording try a Gauge instead");
                }

                _ctr.IncTo(targetValue);
            }
        }

        /// <inheritdoc />
        public void IncrementLabel(double amount, params string[] labelValues)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("Counters may only be incremented. If you need to reduce the number you are recording try a Gauge instead");
            }

            if (labelValues == null || !labelValues.Any())
            {
                throw new ArgumentException("You must provide label values", nameof(labelValues));
            }
            _ctr.WithLabels(labelValues).Inc(amount);
        }

        /// <inheritdoc />
        public void IncrementLabel(params string [] labelValues)
        {
            IncrementLabel(1.0, labelValues);
        }
        
        /// <inheritdoc />
        public void IncrementLabelTo(double targetValue, params string [] labelValues)
        {
            if (labelValues == null || !labelValues.Any())
            {
                throw new ArgumentException("You must provide label values", nameof(labelValues));
            }


            foreach (var label in labelValues)
            {
                var child = _ctr.WithLabels(label);
                lock (child)
                {
                    if (targetValue < child.Value)
                    {
                        throw new InvalidOperationException("You may not increment a counter to a value lower than it's current value. If you need to reduce the number you are recording try a Gauge instead");
                    }

                    child.IncTo(targetValue);
                }
            }
        }
	}
}
