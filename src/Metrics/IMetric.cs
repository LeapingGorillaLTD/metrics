namespace LeapingGorilla.Metrics
{
    /// <summary>
    /// Base interface that all metrics implement
    /// </summary>
    public interface IMetric
    {
        /// <summary>
        /// The name of this metric as it appears in the target collection system (i.e. Prometheus)
        /// </summary>
        public string NameInStorageSystem { get; }

        /// <summary>
        /// Description of what this metric applies to
        /// </summary>
        public string Description { get; }
    }
}
