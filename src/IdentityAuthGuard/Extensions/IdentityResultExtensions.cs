using Core.Wrappers;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IdentityAuthGuard.Extensions
{
    /// <summary>
    /// Extension methods for IdentityResult to provide custom error responses.
    /// </summary>
    internal static class IdentityResultExtensions
    {
        /// <summary>
        /// Converts an <see cref="IdentityResult"/> into a custom <see cref="Response"/> object containing error details.
        /// </summary>
        /// <param name="result">The <see cref="IdentityResult"/> instance containing error information.</param>
        /// <param name="unknownError">A fallback error message if no errors are present in the result.</param>
        /// <returns>A <see cref="Response"/> object with the error code and concatenated error messages.</returns>
        public static Response ErrorResponse(this IdentityResult result, string unknownError = "")
        {
            var errors = result.Errors
                    .Select(e => e.Description?.Trim(' ', '.') ?? string.Empty)
                    .Where(error => !string.IsNullOrWhiteSpace(error))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

            var message = errors.Count > 0
                ? string.Join(". ", errors) + "."
                : unknownError;

            if (!int.TryParse(result?.Errors.First().Code, out int code))
            {
                code = 500;
            }

            return Response.Full(code, message);
        }
    }
}
