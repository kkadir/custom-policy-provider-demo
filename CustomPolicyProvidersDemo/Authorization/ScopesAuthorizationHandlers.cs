using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class ScopesAuthorizationHandlers : AuthorizationHandler<ScopesRequirement>
    {
        private readonly ILogger<ScopesAuthorizationHandlers> _logger;

        public ScopesAuthorizationHandlers(ILogger<ScopesAuthorizationHandlers> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ScopesRequirement requirement)
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
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User == null || requirement == null || string.IsNullOrWhiteSpace(requirement.Scopes))
            {
                return Task.CompletedTask;
            }

            var requirementTokens = requirement.Scopes.Split("|", StringSplitOptions.RemoveEmptyEntries);

            if (requirementTokens?.Any() != true)
            {
                return Task.CompletedTask;
            }

            var expectedRequirements = requirementTokens.ToList();

            if (expectedRequirements.Count == 0)
            {
                return Task.CompletedTask;
            }

            var userScopeClaims = context.User.Claims?.Where(c =>
                string.Equals(c.Type, "scope", StringComparison.OrdinalIgnoreCase));

            foreach (var claim in userScopeClaims ?? Enumerable.Empty<Claim>())
            {
                var match = expectedRequirements
                    .Where(r => string.Equals(r, claim.Value, StringComparison.OrdinalIgnoreCase));

                if (match.Any())
                {
                    context.Succeed(requirement);
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
