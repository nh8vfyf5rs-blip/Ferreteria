using Ferreteria.Models.Segurudad;

namespace Ferreteria.Helpers
{
    public class EffectivePermission
    {
     public static IReadOnlyList<string> FromUser(Usuario usuario)
        {
            var fromRole = usuario.Rol?.RolPermisos?
                .Select(rp => rp.Permiso.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var granted = usuario.UsuarioPermisos?
                .Where(up => up.IsGranted)
                .Select(up => up.Permiso.Name)
                .ToList() ?? new List<string>();

            var denied = usuario.UsuarioPermisos?
                .Where(up => !up.IsGranted)
                .Select(up => up.Permiso.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var permission in granted)
            {
                fromRole.UnionWith(granted);
                fromRole.ExceptWith(denied);
            }

            return fromRole.OrderBy(p => p)
                .ToList();
        }
}
}