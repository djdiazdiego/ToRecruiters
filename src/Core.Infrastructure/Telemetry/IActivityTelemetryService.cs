using System.Diagnostics;

namespace Core.Infrastructure.Telemetry
{
    /// <summary>
    /// Provides methods to start and manage telemetry activities.
    /// </summary>
    public interface IActivityTelemetryService : IDisposable
    {
        /// <summary>
        /// Starts a new telemetry activity with the specified source name, activity name, and kind.
        /// </summary>
        /// <param name="sourceName">The name of the activity source.</param>
        /// <param name="activityName">The name of the activity.</param>
        /// <param name="kind">The kind of activity. Defaults to <see cref="ActivityKind.Client"/>.</param>
        /// <returns>
        /// An <see cref="Activity"/> instance if the activity is started successfully; otherwise, <c>null</c>.
        /// </returns>
        Activity? StartActivity(string sourceName, string activityName, ActivityKind kind = ActivityKind.Client);
    }
}
