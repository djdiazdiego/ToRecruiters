namespace Core.Data.Seeds
{
    /// <summary>
    /// Represents a contract for seeding data into a data source.
    /// </summary>
    public interface ISeed
    {
        /// <summary>
        /// Asynchronously loads seed data into the data source.
        /// </summary>
        /// <param name="provider">The service provider used to resolve dependencies.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SeedAsync(IServiceProvider provider, CancellationToken cancellationToken = default);
    }
}
