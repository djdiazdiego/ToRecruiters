using System;

namespace IdentityAuthGuard.Services.GuidGeneratorService
{
    /// <summary>
    /// Sequential Guid generator
    /// </summary>
    public interface IGuidGeneratorService
    {
        /// <summary>
        /// Generate new sequential Guid
        /// </summary>
        Guid New { get; }
    }
}