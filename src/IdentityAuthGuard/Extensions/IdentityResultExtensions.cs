using Core.Wrappers;
using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Extensions
{
    /// <summary>
    /// Extension methods for IdentityResult to provide custom error responses.
    /// </summary>
    internal static class IdentityResultExtensions
    {
        /// <summary>
        /// Converts an IdentityResult into a custom Response object containing error details.
        /// </summary>
        /// <param name="result">The IdentityResult instance containing error information.</param>
        /// <returns>A Response object with the error code and concatenated error messages.</returns>
        public static Response ErrorResponse(this IdentityResult result)
        {
            // Extract error descriptions, trimming spaces and periods.
            var errors = result.Errors.Select(x => x.Description.Trim([' ', '.'])).ToArray();

            // Combine error messages into a single string.
            var message = string.Join(". ", errors);

            // Attempt to parse the first error code; default to 500 if parsing fails.
            if (!int.TryParse(result?.Errors.First().Code, out int code))
            {
                code = 500;
            }

            // Return a full Response object with the error code and message.
            return Response.Full(code, message);
        }
    }
}
