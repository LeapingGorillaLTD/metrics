namespace LeapingGorilla.Metrics
{
    /// <summary>
    /// Summaries track the trends in events over a 10 minute window.
    /// They track both the number of observations and the sum allowing
    /// averages to be calculated. A summary also calculates configurable
    /// quantiles (think P-values i.e. P50, P95 etc) over the period.
    /// </summary>
    public interface ISummary : IMetric
    {
        /// <summary>
        /// Observe a value for this summary. Observing a value increments the
        /// total, updates the sum and calculates any quantiles we are tracking.
        /// </summary>
        /// <param name="val"></param>
        void Observe(double val);
    }
}
