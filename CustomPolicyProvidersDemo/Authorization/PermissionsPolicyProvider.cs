using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class PermissionsPolicyProvider : IAuthorizationPolicyProvider
    {
        public PermissionsPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder("Bearer").RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (string.IsNullOrWhiteSpace(policyName))
            {
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }

            var policyTokens = policyName.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (policyTokens?.Any() != true)
            {
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }

            var policy = new AuthorizationPolicyBuilder("Bearer");
            var identifier = Guid.NewGuid();

            foreach (var token in policyTokens)
            {
                var pair = token.Split('$', StringSplitOptions.RemoveEmptyEntries);

                if (pair?.Any() != true || pair.Length != 2)
                {
                    return FallbackPolicyProvider.GetPolicyAsync(policyName);
                }

                IAuthorizationRequirement requirement = (pair[0]) switch
                {
                    PermissionsAttribute.PermissionsGroup => new PermissionsRequirement(pair[1], identifier),
                    PermissionsAttribute.RolesGroup => new RolesRequirement(pair[1], identifier),
                    PermissionsAttribute.ScopesGroup => new ScopesRequirement(pair[1], identifier),
                    _ => null,
                };

                if (requirement == null)
                {
                    return FallbackPolicyProvider.GetPolicyAsync(policyName);
                }

                policy.AddRequirements(requirement);
            }

            return Task.FromResult(policy.Build());
        }
    }
}
