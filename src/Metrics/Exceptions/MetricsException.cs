using System;
using System.Diagnostics.CodeAnalysis;

namespace LeapingGorilla.Metrics.Exceptions
{
    [ExcludeFromCodeCoverage]
    public abstract class MetricsException : Exception
    {
        protected MetricsException(string? message) : base(message)
        {
        }

        protected MetricsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
