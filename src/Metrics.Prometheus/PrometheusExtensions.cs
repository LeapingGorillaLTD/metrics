using Prometheus;

namespace LeapingGorilla.Metrics.Prometheus
{
    /// <summary>Extensions specific to the Prometheus metrics implementation</summary>
    public static class PrometheusExtensions
    {
        /// <summary>
        /// Convert this SummaryObjective to a <see cref="QuantileEpsilonPair"/> which
        /// is natively used by the underlying Prometheus library. This method should
        /// not be called outside of the LeapingGorilla.Metrics.Prometheus library.
        /// </summary>
        /// <returns></returns>
        internal static QuantileEpsilonPair ToQuantileEpsilonPair(this SummaryObjective objective)
        {
            return new QuantileEpsilonPair(objective.Quantile, objective.ErrorMargin);
        }
    }
}