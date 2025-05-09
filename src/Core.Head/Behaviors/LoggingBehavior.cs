using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Core.Head.Behaviors
{
    /// <summary>
    /// A pipeline behavior in the MediatR pipeline for logging the handling of requests and responses.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
            where TRequest : notnull, IBaseRequest
            where TResponse : notnull
    {
        /// <summary>
        /// Handles the request and logs the start, end, and any performance or error details.
        /// </summary>
        /// <param name="request">The request being handled.</param>
        /// <param name="next">The next delegate in the pipeline.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The response from the next delegate in the pipeline.</returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handling request {RequestType} with response {ResponseType}. Request data: {RequestData}",
                typeof(TRequest).Name, typeof(TResponse).Name, request);

            var stopwatch = Stopwatch.StartNew();
            TResponse response;

            try
            {
                response = await next(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[ERROR] An exception occurred while handling request {RequestType}.", typeof(TRequest).Name);
                throw;
            }
            finally
            {
                stopwatch.Stop();
            }

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > 3000)
            {
                logger.LogWarning("[PERFORMANCE] Handling request {RequestType} took {ElapsedMilliseconds} ms.", typeof(TRequest).Name, elapsedMilliseconds);
            }

            logger.LogInformation("[END] Handled request {RequestType} with response {ResponseType}. Time taken: {ElapsedMilliseconds} ms.",
                typeof(TRequest).Name, typeof(TResponse).Name, elapsedMilliseconds);

            return response;
        }
    }
}
