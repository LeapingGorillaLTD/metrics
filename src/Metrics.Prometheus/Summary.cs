namespace LeapingGorilla.Metrics.Prometheus
{
    /// <inheritdoc cref="ISummary" />
    public class Summary : Metric, ISummary
    {
        private readonly global::Prometheus.Summary _summary;

        public Summary(global::Prometheus.Summary summary, string nameInStorageSystem, string description) 
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