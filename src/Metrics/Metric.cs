namespace LeapingGorilla.Metrics
{
    /// <inheritdoc cref="IMetric" />
    public abstract class Metric : IMetric
    {
        /// <inheritdoc />
        public string NameInStorageSystem { get; }

        /// <inheritdoc />
        public string Description { get; }

        protected Metric(string nameInStorageSystem, string description)
        {
            NameInStorageSystem = nameInStorageSystem;
            Description = description;
        }
    }
}
