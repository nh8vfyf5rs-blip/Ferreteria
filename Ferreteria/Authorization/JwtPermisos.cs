using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Ferreteria.Extensions;

namespace Ferreteria.Authorization
{
    public class JwtPermiso

    {
        public class PermisoRequirement(string permission) : IAuthorizationRequirement
        {
            public string Permission { get; } = permission;

        }

        public class PermissionAuthorizationHandler : AuthorizationHandler<PermisoRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermisoRequirement requirement)
            {
                if (context.User.HasPermission(requirement.Permission))
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
        }



        public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
        {
            public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
            {
                var policy = await base.GetPolicyAsync(policyName);
                if (policy is not null)
                    return policy;
                return new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermisoRequirement(policyName))
                    .Build();
            }
        }
    }
}
