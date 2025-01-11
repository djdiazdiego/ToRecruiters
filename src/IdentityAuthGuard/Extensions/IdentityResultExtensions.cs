using Core.Head.Wrappers;
using Microsoft.AspNetCore.Identity;

namespace IdentityAuthGuard.Extensions
{
    internal static class IdentityResultExtensions
    {
        public static Response ErrorResponse(this IdentityResult result)
        {
            var errors = result.Errors.Select(x => x.Description.Trim([' ', '.'])).ToArray();
            var message = string.Join(". ", errors);

            if (!int.TryParse(result?.Errors.First().Code, out int code))
            {
                code = 500;
            }

            return Response.Full(code, message);
        }
    }
}
