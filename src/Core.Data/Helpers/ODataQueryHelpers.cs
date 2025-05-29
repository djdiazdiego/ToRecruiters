using AutoMapper;
using Core.BaseModels;
using Core.Wrappers;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.Data.Helpers
{
    public static class ODataQueryHelpers
    {
        /// <summary>
        /// Retrieves paginated data from a queryable source, applying OData query options, search, and mapping to a response type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity in the queryable source.</typeparam>
        /// <typeparam name="TResponse">The type of the response object.</typeparam>
        /// <param name="query">The queryable source of data.</param>
        /// <param name="oData">The OData query options to apply.</param>
        /// <param name="mapper">The AutoMapper instance for mapping entities to response objects.</param>
        /// <param name="buildSearch">An optional function to build search expressions based on a search string.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="PageResponse{TResponse}"/> containing the paginated data.</returns>
        public static async Task<PageResponse<TResponse>> GetPageDataAsync<TEntity, TResponse>(
            IQueryable<TEntity> query,
            ODataQueryOptions<TEntity> oData,
            IMapper mapper,
            Func<string, Expression<Func<TEntity, bool>>>? buildSearch = null,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
            where TResponse : class
        {
            // Apply includes for navigation properties specified in the OData $expand option
            if (!string.IsNullOrWhiteSpace(oData.SelectExpand?.RawExpand))
            {
                var includes = oData.SelectExpand.RawExpand
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(include => include.Trim());

                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            var querySettings = new ODataQuerySettings
            {
                // Optimize for performance
                HandleNullPropagation = HandleNullPropagationOption.False
            };

            // Apply filtering based on the OData $filter option
            if (oData.Filter != null)
            {
                // Apply the OData $filter option to the query   
                query = (IQueryable<TEntity>)oData.Filter.ApplyTo(query, querySettings);
            }

            // Apply search functionality if a search expression builder is provided
            if (buildSearch is not null && !string.IsNullOrWhiteSpace(oData.Search?.RawValue))
            {
                var searchExpression = buildSearch(oData.Search.RawValue);
                if (searchExpression is not null)
                {
                    query = query.Where(searchExpression);
                }
            }

            // Count the total number of records after applying filters
            var totalRecords = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            // Apply sorting based on the OData $orderby option
            if (oData.OrderBy != null)
            {
                query = oData.OrderBy.ApplyTo(query);
            }

            // Apply pagination: skip records based on the OData $skip option
            if (oData.Skip != null)
            {
                query = oData.Skip.ApplyTo(query, querySettings);
            }

            // Apply pagination: limit the number of records based on the OData $top option
            if (oData.Top != null)
            {
                query = oData.Top.ApplyTo(query, querySettings);
            }

            // Calculate page size and page number
            int pageSize = oData.Top?.Value ?? totalRecords;
            int pageNumber = oData.Skip?.Value / pageSize + 1 ?? 1;

            // Retrieve the filtered, sorted, and paginated data
            var entities = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

            // Map the entities to the response type
            var response = mapper.Map<List<TResponse>>(entities);

            // Return the paginated response
            return PageResponse<TResponse>.Ok(
                response,
                pageNumber,
                pageSize,
                totalRecords);
        }
    }
}
