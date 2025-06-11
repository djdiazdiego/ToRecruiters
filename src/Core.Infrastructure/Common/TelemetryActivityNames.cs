namespace Core.Infrastructure.Common
{
    /// <summary>
    /// Provides constant names for telemetry activities used throughout the application.
    /// </summary>
    public static class TelemetryActivityNames
    {
        /// <summary>
        /// The name of the activity for database access.
        /// </summary>
        public const string DATABASE_ACCESS = "DatabaseAccess";

        /// <summary>
        /// The name of the activity for the HTTP request.
        /// </summary>
        public const string HTTP_REQUEST = "HttpRequest";

        /// <summary>
        /// The name of the activity for the HTTP response.
        /// </summary>
        public const string HTTP_RESPONSE = "HttpResponse";

        /// <summary>
        /// The name of the activity for integration events.
        /// </summary>
        public const string INTEGRATION_EVENT = "IntegrationEvent";
    }
}
