using System;

namespace Core.BaseDTOs
{
    public abstract class DTO
    {
        /// <summary>
        /// Creation date
        /// </summary>
        public DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// Last update date
        /// </summary>
        public DateTimeOffset? LastUpdateDate { get; set; }

    }
}
