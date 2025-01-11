namespace Core.Head.Wrappers
{
    public interface IResponse
    {
        int Code { get; }
        string ErrorMessage { get; }
    }

    public interface IResponse<TData> : IResponse where TData : class
    {
        TData Data { get; }
    }

    public interface IPageResponse<TData> : IResponse where TData : class
    {
        List<TData> Data { get; }
    }
}
