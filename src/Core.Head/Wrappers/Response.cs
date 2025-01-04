using System.Runtime.Serialization;

namespace Core.Head.Wrappers
{
    /// <summary>
    /// Custom response
    /// </summary>
    [DataContract]
    public class Response(int code, string errorMessage) : IResponse
    {
        public Response() : this(200, string.Empty) { }

        [DataMember]
        public int Code { get; set; } = code;

        [DataMember]
        public string ErrorMessage { get; set; } = errorMessage;

        [IgnoreDataMember]
        public static Response Ok => new();

        public static Response Full(int code, string errorMessage) => new(code, errorMessage);
    }

    /// <summary>
    /// Custom response with extra data
    /// </summary>
    [DataContract]
    public sealed class Response<TData>(int code, string errorMessage, TData data) :
        Response(code, errorMessage), IResponse<TData> where TData : class
    {
        public Response(TData data) : this(200, string.Empty, data) { }

        public Response() : this(200, string.Empty, default) { }

        [DataMember]
        public TData Data { get; set; } = data;

        public static new Response<TData> Ok(TData data) => new(data);

        public static Response Full(int code, string errorMessage, TData data) =>
            new Response<TData>(code, errorMessage, data);
    }

    [DataContract]
    public sealed class PageResponse<TData>(
        int code,
        string errorMessage,
        List<TData> data,
        int pageNumber,
        int pageSize,
        int totalRecords) :
        Response(code, errorMessage),
        IPageResponse<TData> where TData : class
    {
        public PageResponse(
            List<TData> data,
            int pageNumber,
            int pageSize,
            int totalRecords) : this(200, string.Empty, data, pageNumber, pageSize, totalRecords) { }

        [DataMember]
        public List<TData> Data { get; set; } = data ?? [];
        [DataMember]
        public int PageNumber { get; set; } = pageNumber;
        [DataMember]
        public int PageSize { get; set; } = pageSize;
        [DataMember]
        public int TotalRecords { get; set; } = totalRecords;
        [DataMember]
        public int TotalPages { get; set; } = (int)Math.Ceiling((double)totalRecords / pageSize);

        public static new PageResponse<TData> Ok(
            List<TData> data,
            int pageNumber,
            int pageSize,
            int totalRecords) => new(data, pageNumber, pageSize, totalRecords);

        public static PageResponse<TData> Full(
            int code,
            string errorMessage,
            List<TData> data,
            int pageNumber,
            int pageSize,
            int totalRecords) => new(code, errorMessage, data, pageNumber, pageSize, totalRecords);
    }
}
