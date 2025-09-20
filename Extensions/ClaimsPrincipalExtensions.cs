using System.Security.Claims;

namespace MOJTaskDemo.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal user, string code) =>
            user?.Identity?.IsAuthenticated == true &&
            user.Claims.Any(c => c.Type == "perm" && c.Value == code);
    }
}
