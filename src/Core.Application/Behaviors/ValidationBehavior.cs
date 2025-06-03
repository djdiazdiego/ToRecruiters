using Core.Application.CQRS;
using FluentValidation;
using MediatR;

namespace Core.Application.Behaviors
{
    /// <summary>
    /// Represents a behavior in the MediatR pipeline that performs validation on the incoming request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IBaseCommand
    {
        /// <summary>
        /// Handles the validation of the request and invokes the next delegate in the pipeline.
        /// </summary>
        /// <param name="request">The incoming request to validate.</param>
        /// <param name="next">The next delegate in the pipeline.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The response from the next delegate in the pipeline.</returns>
        /// <exception cref="ValidationException">Thrown when validation failures are detected.</exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next(cancellationToken);
        }
    }
}