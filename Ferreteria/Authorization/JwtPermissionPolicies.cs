using Ferreteria.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Ferreteria.Authorization
{
    public class JwtPermissionPolicies
    {
        public class PermissionRequirement(string permission) : IAuthorizationRequirement
        {
            public string Permission { get; } = permission;
        }

        public class PermissionAuthorizationHandler
            : AuthorizationHandler<PermissionRequirement>
        {
            protected override Task HandleRequirementAsync(
                AuthorizationHandlerContext context,
                PermissionRequirement requirement)
            {
                if (context.User.HasPermission(requirement.Permission))
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }

        public class PermissionPolicyProvider(
            IOptions<AuthorizationOptions> options)
            : DefaultAuthorizationPolicyProvider(options)
        {
            public override async Task<AuthorizationPolicy?> GetPolicyAsync(
                string policyName)
            {
                var policy = await base.GetPolicyAsync(policyName);

                if (policy is not null)
                    return policy;

                return new AuthorizationPolicyBuilder()
                    .AddRequirements(
                        new PermissionRequirement(policyName))
                    .Build();
            }
        }
    }
    }
