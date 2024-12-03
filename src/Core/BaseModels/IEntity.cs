using System;

namespace Core.BaseModels
{
    public interface IEntity
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        object Id { get; }

        /// <summary>
        /// Creation date
        /// </summary>
        DateTimeOffset CreationDate { get; }

        /// <summary>
        /// Last update date
        /// </summary>
        DateTimeOffset? LastUpdateDate { get; }

        /// <summary>
        /// Indicate entity is not initialized yet
        /// </summary>
        /// <returns></returns>
        bool IsTransient();
    }
}
