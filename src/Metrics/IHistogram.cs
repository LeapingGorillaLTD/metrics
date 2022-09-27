namespace LeapingGorilla.Metrics
{
    /// <summary>
    /// Samples observations and counts them in configurable buckets. Also provides
    /// a Sum and Count of all observed values. Quantiles (i.e. P values) can be
    /// calculated on the server by using the histogram_quantile() function.
    ///
    /// See https://prometheus.io/docs/concepts/metric_types/#histogram for more
    /// </summary>
    public interface IHistogram : IMetric
    {
        /// <summary>Total number of items observed by this Histogram</summary>
        long Count { get; }

        /// <summary>Total sum of all items observed by this histogram</summary>
        double Sum { get; }

        /// <summary>
        /// Observe a value for this histogram
        /// </summary>
        /// <param name="val">The value to observe</param>
        void Observe(double val);

        /// <summary>
        /// Observe a number of values for this histogram. The bucket corresponding
        /// to <see cref="val"/> will have <see cref="count"/> items added.
        /// </summary>
        /// <param name="val">The value to observe</param>
        /// <param name="count">The number of items to add to the bucket</param>
        void Observe(double val, long count);
    }
}
