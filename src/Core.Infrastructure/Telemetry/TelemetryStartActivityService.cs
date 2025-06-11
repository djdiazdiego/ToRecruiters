using Core.Infrastructure.Common;
using System.Diagnostics;

namespace Core.Infrastructure.Telemetry
{
    /// <summary>
    /// Provides methods to start and manage telemetry activities.
    /// </summary>
    public class ActivityTelemetryService : IActivityTelemetryService, IDisposable
    {
        private readonly Dictionary<string, ActivitySource> _sources;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityTelemetryService"/> class.
        /// </summary>
        public ActivityTelemetryService()
        {
            _sources = new Dictionary<string, ActivitySource>
            {
                { TelemetryActivityNames.DATABASE_ACCESS, new ActivitySource(TelemetryActivityNames.DATABASE_ACCESS) },
                { TelemetryActivityNames.HTTP_REQUEST, new ActivitySource(TelemetryActivityNames.HTTP_REQUEST) },
                { TelemetryActivityNames.HTTP_RESPONSE, new ActivitySource(TelemetryActivityNames.HTTP_RESPONSE) },
                { TelemetryActivityNames.INTEGRATION_EVENT, new ActivitySource(TelemetryActivityNames.INTEGRATION_EVENT) }
            };
        }

        /// <inheritdoc/>
        public Activity? StartActivity(string sourceName, string activityName, ActivityKind kind = ActivityKind.Client)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(ActivityTelemetryService));

            return _sources.TryGetValue(sourceName, out var activitySource) ?
                   activitySource.StartActivity(activityName) : null;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed) return;

            foreach (var source in _sources.Values)
            {
                source.Dispose();
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
