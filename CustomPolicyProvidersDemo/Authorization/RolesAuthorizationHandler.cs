using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesRequirement>
    {
        private readonly ILogger<RolesAuthorizationHandler> _logger;

        public RolesAuthorizationHandler(ILogger<RolesAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RolesRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return Task.CompletedTask;
            }

            if (context.HasSucceeded)
            {
                return Task.CompletedTask;
            }

            if (context.User.IsInRole("Super Admin"))
            {
                Utility.Succeed(context, requirement.Identifier);
                return Task.CompletedTask;
            }

            if (context.User == null || requirement == null || string.IsNullOrWhiteSpace(requirement.Roles))
            {
                return Task.CompletedTask;
            }

            var requirementTokens = requirement.Roles.Split("|", StringSplitOptions.RemoveEmptyEntries);

            if (requirementTokens?.Any() != true)
            {
                return Task.CompletedTask;
            }

            var expectedRequirements = requirementTokens.ToList();

            if (expectedRequirements.Count == 0)
            {
                return Task.CompletedTask;
            }

            var userRoleClaims = context.User.Claims?.Where(c =>
                string.Equals(c.Type, "role", StringComparison.OrdinalIgnoreCase)
                || string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase));

            foreach (var claim in userRoleClaims ?? Enumerable.Empty<Claim>())
            {
                var match = expectedRequirements
                    .Where(r => string.Equals(r, claim.Value, StringComparison.OrdinalIgnoreCase));

                if (match.Any())
                {
                    Utility.Succeed(context, requirement.Identifier);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
