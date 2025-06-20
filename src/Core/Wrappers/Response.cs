﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core.Wrappers
{
    /// <summary>
    /// Represents a basic response with a status code and an error message.
    /// </summary>
    [DataContract]
    public class Response : IResponse
    {
        /// <inheritdoc />
        public Response(int code, string message)
        {
            Status = code;
            Details = message;
        }

        /// <inheritdoc />
        public Response() : this(200, string.Empty) { }

        /// <inheritdoc />
        [DataMember]
        public int Status { get; set; }

        /// <inheritdoc />
        [DataMember]
        public string Details { get; set; }

        /// <summary>
        /// Gets a default successful response (200 OK).
        /// </summary>
        [IgnoreDataMember]
        public static Response Ok => new Response();

        /// <summary>
        /// Creates a new response with a specific code and error message.
        /// </summary>
        /// <param name="code">The status code of the response.</param>
        /// <param name="message">The error message of the response.</param>
        /// <returns>A new <see cref="Response"/> instance.</returns>
        public static Response Full(int code, string message) => new Response(code, message);
    }

    /// <summary>
    /// Represents a response with additional data.
    /// </summary>
    /// <typeparam name="TData">The type of the data included in the response.</typeparam>
    [DataContract]
    public sealed class Response<TData> : Response, IResponse<TData> where TData : class
    {
        /// <inheritdoc />
        public Response(int code, string message, TData data)
        {
            Status = code;
            Details = message;
            Data = data;
        }

        /// <inheritdoc />
        public Response(TData data) : this(200, string.Empty, data) { }

        /// <inheritdoc />
        public Response() : this(200, string.Empty, default) { }

        /// <inheritdoc />
        [DataMember]
        public TData Data { get; set; }

        /// <inheritdoc />
        public static new Response<TData> Ok(TData data) => new Response<TData>(data);

        /// <inheritdoc />
        public static Response Full(int code, string message, TData data) =>
            new Response<TData>(code, message, data);
    }

    /// <summary>
    /// Represents a paginated response with additional data.
    /// </summary>
    /// <typeparam name="TData">The type of the data included in the response.</typeparam>
    [DataContract]
    public sealed class PageResponse<TData> : Response, IPageResponse<TData> where TData : class
    {
        /// <inheritdoc />
        public PageResponse(
            int code,
            string message,
            List<TData> data,
            int pageNumber,
            int pageSize,
            int totalRecords)
        {
            Status = code;
            Details = message;
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize == 0 ? PageSize : pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
        }

        /// <inheritdoc />
        public PageResponse(
            List<TData> data,
            int pageNumber,
            int pageSize,
            int totalRecords) : this(200, string.Empty, data, pageNumber, pageSize, totalRecords) { }

        /// <inheritdoc />
        [DataMember]
        public List<TData> Data { get; set; } = new List<TData>();

        /// <inheritdoc />
        [DataMember]
        public int PageNumber { get; set; } = 1;

        /// <inheritdoc />
        [DataMember]
        public int PageSize { get; set; } = 10;

        /// <inheritdoc />
        [DataMember]
        public int TotalRecords { get; set; } = 0;

        /// <inheritdoc />
        [DataMember]
        public int TotalPages { get; set; } = 0;

        /// <inheritdoc />
        public static new PageResponse<TData> Ok(
            List<TData> data,
            int pageNumber,
            int pageSize,
            int totalRecords) => new PageResponse<TData>(data, pageNumber, pageSize, totalRecords);

        /// <inheritdoc />
        public static PageResponse<TData> Full(
            int code,
            string message,
            List<TData> data,
            int pageNumber,
            int pageSize,
            int totalRecords) => new PageResponse<TData>(code, message, data, pageNumber, pageSize, totalRecords);
    }
}
