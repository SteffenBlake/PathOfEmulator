using System;
using System.Security.Claims;

namespace PathOfEmulator.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetLoggedInUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetLoggedInUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.Name);
        }
    }
}
