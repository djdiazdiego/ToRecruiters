namespace Core.Data.Seeds
{
    public interface ISeed
    {
        /// <summary>
        /// Load seed data
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SeedAsync(IServiceProvider provider, CancellationToken cancellationToken = default);
    }
}
