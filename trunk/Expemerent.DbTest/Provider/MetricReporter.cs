using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expemerent.DbTest.Provider
{
    /// <summary>
    /// Metric report result
    /// </summary>
    internal class Metric
    {
        /// <summary>
        /// Operation title
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets operation duration
        /// </summary>
        public TimeSpan? Duration { get; set; }
    }

    /// <summary>
    /// Metrics reported implementation
    /// </summary>
    internal class MetricReporter
    {
        /// <summary>
        /// Collection of reported metrics
        /// </summary>
        private List<Metric> _metrics = new List<Metric>();

        /// <summary>
        /// Begins operation and reports it on <see cref="IDisposable.Dispose"/> call
        /// </summary>
        public IDisposable BeginOperation(String operationName)
        {
            var start = DateTime.Now;
            Action complete = delegate
            {
                Report(operationName, DateTime.Now.Subtract(start));
            };

            return new DisposableAction(complete);
        }

        /// <summary>
        /// Reports metrics for a simple operation
        /// </summary>
        private void Report(String operationName, TimeSpan duration)
        {
            _metrics.Add(new Metric() { Operation = operationName, Duration = duration });
        }

        /// <summary>
        /// Gets list of collected metrics
        /// </summary>
        public IList<Metric> Metrics { get { return _metrics.AsReadOnly(); } }
    }
}
