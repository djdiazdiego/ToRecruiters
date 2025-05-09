namespace IdentityAuthGuard.Services.GuidGeneratorServices
{
    /// <summary>
    /// Provides functionality to generate sequential GUIDs.
    /// </summary>
    public interface IGuidGeneratorService
    {
        /// <summary>
        /// Gets a new sequential GUID.
        /// </summary>
        Guid New { get; }
    }
}