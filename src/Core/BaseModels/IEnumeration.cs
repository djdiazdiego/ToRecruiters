using System;

namespace Core.BaseModels
{
    /// <summary>
    /// Represents an enumeration interface that extends the base entity and provides comparison functionality.
    /// </summary>
    public interface IEnumeration : IEntity, IComparable
    {
        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sets the name of the field.
        /// </summary>
        /// <param name="name">The name to set.</param>
        void SetName(string name);
    }
}
