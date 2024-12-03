using System.Runtime.Serialization;

namespace Core.Head.Wrappers
{
    /// <summary>
    /// Custom response
    /// </summary>
    [DataContract]
    public class Response : IResponse
    {
        public Response(bool isSuccess, int code, string errorMessage)
        {
            Succeeded = isSuccess;
            Code = code;
            ErrorMessage = errorMessage;
        }

        public Response(int code, string errorMessage) : this(false, code, errorMessage) { }

        public Response() : this(true, 200, string.Empty) { }

        [DataMember]
        public bool Succeeded { get; set; }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [IgnoreDataMember]
        public static Response Ok => new Response();

        public static Response Error(int code, string errorMessage) => new Response(code, errorMessage);

        public static Response Full(bool isSuccess, int code, string errorMessage) => new Response(isSuccess, code, errorMessage);
    }

    /// <summary>
    /// Custom response with extra data
    /// </summary>
    [DataContract]
    public sealed class Response<TData> : Response, IResponse<TData> where TData : class

    {
        public Response(bool isSuccess, int code, string errorMessage, TData data) : base(isSuccess, code, errorMessage)
        {
            Data = data;
        }

        public Response(int code, string errorMessage, TData data) : this(false, code, errorMessage, data) { }

        public Response(TData data) : this(true, 200, string.Empty, data) { }

        public Response() : this(true, 200, string.Empty, default) { }

        [DataMember]
        public TData Data { get; set; }

        [IgnoreDataMember]
        public static Response<TData> NullDataOk => new Response<TData>();

        public static Response<TData> DataOk(TData data) => new Response<TData>(data);

        public static Response Error(int code, string errorMessage, TData data) =>
            new Response<TData>(code, errorMessage, data);

        public static Response Full(bool isSuccess, int code, string errorMessage, TData data) =>
            new Response<TData>(isSuccess, code, errorMessage, data);
    }
}
