namespace Core.Head.Wrappers
{
    public interface IResponse
    {
        public bool Succeeded { get; }
        int Code { get; }
        string ErrorMessage { get; }
    }

    public interface IResponse<TData> : IResponse where TData : class
    {
        TData Data { get; }
    }
}
