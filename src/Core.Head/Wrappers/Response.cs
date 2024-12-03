using System.Runtime.Serialization;

namespace Core.Head.Wrappers
{
    /// <summary>
    /// Custom response
    /// </summary>
    [DataContract]
    public class Response(bool isSuccess, int code, string errorMessage) : IResponse
    {
        public Response(int code, string errorMessage) : this(false, code, errorMessage) { }

        public Response() : this(true, 200, string.Empty) { }

        [DataMember]
        public bool Succeeded { get; set; } = isSuccess;

        [DataMember]
        public int Code { get; set; } = code;

        [DataMember]
        public string ErrorMessage { get; set; } = errorMessage;

        [IgnoreDataMember]
        public static Response Ok => new();

        public static Response Error(int code, string errorMessage) => new(code, errorMessage);

        public static Response Full(bool isSuccess, int code, string errorMessage) => new(isSuccess, code, errorMessage);
    }

    /// <summary>
    /// Custom response with extra data
    /// </summary>
    [DataContract]
    public sealed class Response<TData>(bool isSuccess, int code, string errorMessage, TData data) : 
        Response(isSuccess, code, errorMessage), IResponse<TData> where TData : class
    {
        public Response(int code, string errorMessage, TData data) : this(false, code, errorMessage, data) { }

        public Response(TData data) : this(true, 200, string.Empty, data) { }

        public Response() : this(true, 200, string.Empty, default) { }

        [DataMember]
        public TData Data { get; set; } = data;

        [IgnoreDataMember]
        public static Response<TData> NullDataOk => new();

        public static Response<TData> DataOk(TData data) => new(data);

        public static Response Error(int code, string errorMessage, TData data) =>
            new Response<TData>(code, errorMessage, data);

        public static Response Full(bool isSuccess, int code, string errorMessage, TData data) =>
            new Response<TData>(isSuccess, code, errorMessage, data);
    }
}
