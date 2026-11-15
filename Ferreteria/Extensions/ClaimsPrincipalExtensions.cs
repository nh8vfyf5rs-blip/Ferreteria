using System.Security.Claims;

namespace Ferreteria.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
            public const string PermissionClaimType = "Permiso";

            public static bool HasPermission(this ClaimsPrincipal user, string permiso)
            {
                return user.Claims.Any(c => c.Type == PermissionClaimType && string.Equals(c.Value, permiso, StringComparison.OrdinalIgnoreCase));
            }
            public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permiso)

            {
                return permiso.Any(user.HasPermission);
            }

            public static IReadOnlyList<string> GetPermission(this ClaimsPrincipal user)
            {
                return user.Claims
                  .Where(c => c.Type == PermissionClaimType)
                  .Select(c => c.Value)
                  .OrderBy(p => p)
                  .ToList();
            }
        }
    }


