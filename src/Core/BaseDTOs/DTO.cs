using System;

namespace Core.BaseDTOs
{
    /// <summary>
    /// Represents a base Data Transfer Object (DTO) with common properties.
    /// </summary>
    public abstract class DTO
    {
        /// <summary>
        /// Gets or sets the creation date of the DTO.
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the last update date of the DTO. 
        /// This value is null if the DTO has not been updated.
        /// </summary>
        public DateTimeOffset? LastUpdateDate { get; set; }
    }
}
