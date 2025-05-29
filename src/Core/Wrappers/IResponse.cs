using System.Collections.Generic;

namespace Core.Wrappers
{
    /// <summary>
    /// Represents a standard response with a status code and an error message.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets the status code of the response.
        /// </summary>
        int Status { get; }

        /// <summary>
        /// Gets the message details of the response, if any.
        /// </summary>
        string Details { get; }
    }

    /// <summary>
    /// Represents a standard response with a status code, error message, and data payload.
    /// </summary>
    /// <typeparam name="TData">The type of the data payload.</typeparam>
    public interface IResponse<TData> : IResponse where TData : class
    {
        /// <summary>
        /// Gets the data payload of the response.
        /// </summary>
        TData Data { get; }
    }

    /// <summary>
    /// Represents a paginated response with a status code, error message, and a list of data items.
    /// </summary>
    /// <typeparam name="TData">The type of the data items.</typeparam>
    public interface IPageResponse<TData> : IResponse where TData : class
    {
        /// <summary>
        /// Gets the list of data items in the response.
        /// </summary>
        List<TData> Data { get; }
    }
}
