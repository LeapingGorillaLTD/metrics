﻿namespace LeapingGorilla.Metrics.Prometheus
{
    /// <inheritdoc cref="IHistogram" />
    public class Histogram : Metric, IHistogram
    {
        private readonly global::Prometheus.Histogram _histogram;

        public Histogram(global::Prometheus.Histogram histogram, string nameInStorageSystem, string description) 
            : base(nameInStorageSystem, description)
        {
            _histogram = histogram;
        }

        /// <inheritdoc />
        public long Count => _histogram.Count;

        /// <inheritdoc />
        public double Sum => _histogram.Sum;

        /// <inheritdoc />
        public void Observe(double val)
        {
            _histogram.Observe(val);
        }
        
        /// <inheritdoc />
        public void Observe(double val, long count)
        {
            _histogram.Observe(val, count);
        }
    }
}
