namespace LeapingGorilla.Metrics
{
    /// <inheritdoc cref="ISummary" />
    public class Summary : Metric, ISummary
    {
        private readonly Prometheus.Summary _summary;

        public Summary(Prometheus.Summary summary, string nameInStorageSystem, string description) 
            : base(nameInStorageSystem, description)
        {
            _summary = summary;
        }

        /// <inheritdoc />
        public void Observe(double val)
        {
            _summary.Observe(val);
        }
    }
}
