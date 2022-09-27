using System;
using System.Diagnostics.CodeAnalysis;
using Prometheus;

namespace LeapingGorilla.Metrics
{
    /// <summary>
    /// Represents an objective that should be tracked for a summary.
    /// An objective is a P-value or quantile with a given error
    /// allowance. The Quantile should have a value between 0 and 1.
    /// 0.5 represents the median (or P50) value - the value that
    /// 50% of all observed values are equal to or less than. 0.95 would
    /// be the 95th Percentile where 95% of all observations would be
    /// equal to or lower.
    ///
    /// For more see https://en.wikipedia.org/wiki/Percentile
    /// </summary>
    public readonly struct SummaryObjective
    {
        /// <summary>
        /// A Quantile represents a single p-value. The Quantile should have a value
        /// between 0 and 1. 0.5 represents the median (or P50) value - the value that
        /// 50% of all observed values are equal to or less than. 0.95 would be the
        /// 95th Percentile where 95% of all observations would be equal to or lower.
        /// </summary>
        public double Quantile { get; }


        /// <summary>
        /// The permissible error margin for this objective. A value of 0.05 would have
        /// an acceptable variance of ±5. If we had a Quantile of 50 and an ErrorMargin
        /// of 0.05 then we would allow the p50 value to report in the range of p45-p55.
        /// Lower ErrorMargins take more resources (memory & CPU) to calculate.
        /// </summary>
        public double ErrorMargin { get; }

        /// <summary>
        /// Convert this SummaryObjective to a <see cref="QuantileEpsilonPair"/> which
        /// is natively used by the underlying Prometheus library. This method should
        /// not be called outside of the LeapingGorilla.Metrics library.
        /// </summary>
        /// <returns></returns>
        internal QuantileEpsilonPair ToQuantileEpsilonPair()
        {
            return new QuantileEpsilonPair(Quantile, ErrorMargin);
        }
        
        /// <summary>
        /// Create a new summary objective with the given Quantile and Error Margin
        /// </summary>
        /// <param name="quantile">Ranges from 0 -> 1, represents a P-Value i.e. 0.5 is P50, 0.99 is P95</param>
        /// <param name="errorMargin">
        /// Acceptable margin of error when calculating p-values. 0.01 represents 1% so for a quantile
        /// of P50 a value could be bucketed as P49-P51
        /// </param>
        [ExcludeFromCodeCoverage]
        public SummaryObjective(double quantile, double errorMargin)
        {
            if (quantile <= 0 || quantile > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(quantile), $"Quantile must have a value greater than 0 and less than or equal to 1 (Received: {quantile})");
            }

            Quantile = quantile;
            ErrorMargin = errorMargin;
        }
    }
}
