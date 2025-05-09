using System;

namespace Core.BaseModels
{
    /// <summary>
    /// Represents a base interface for all entities in the application.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets the unique identifier of the entity.
        /// </summary>
        /// <value>
        /// The unique identifier of the entity, which can be of any type.
        /// </value>
        object Id { get; }

        /// <summary>
        /// Gets the date and time when the entity was created.
        /// </summary>
        /// <value>
        /// A <see cref="DateTimeOffset"/> representing the creation date and time of the entity.
        /// </value>
        DateTimeOffset CreationDate { get; }

        /// <summary>
        /// Gets the date and time when the entity was last updated, or <c>null</c> if it has not been updated.
        /// </summary>
        /// <value>
        /// A nullable <see cref="DateTimeOffset"/> representing the last update date and time of the entity.
        /// </value>
        DateTimeOffset? LastUpdateDate { get; }

        /// <summary>
        /// Determines whether the entity is transient, meaning it has not been persisted to a data store.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the entity is transient; otherwise, <c>false</c>.
        /// </returns>
        bool IsTransient();
    }
}
