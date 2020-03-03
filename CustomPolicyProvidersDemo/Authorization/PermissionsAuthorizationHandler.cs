using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
    {
        private readonly ILogger<PermissionsAuthorizationHandler> _logger;

        public PermissionsAuthorizationHandler(ILogger<PermissionsAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionsRequirement requirement)
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

            if (context.User == null || requirement == null || string.IsNullOrWhiteSpace(requirement.Permissions))
            {
                return Task.CompletedTask;
            }

            var requirementTokens = requirement.Permissions.Split("|", StringSplitOptions.RemoveEmptyEntries);

            if (requirementTokens?.Any() != true)
            {
                return Task.CompletedTask;
            }

            List<(string, string, string)> expectedRequirements = new List<(string, string, string)>();

            foreach (var token in requirementTokens)
            {
                var separatedTokens = token.Split(":");

                if (separatedTokens?.Any() != true || separatedTokens.Length != 3)
                {
                    return Task.CompletedTask;
                }

                expectedRequirements.Add((separatedTokens[0], separatedTokens[1], separatedTokens[2]));
            }

            if (expectedRequirements.Count == 0)
            {
                return Task.CompletedTask;
            }

            var userPermissionClaims = context.User.Claims?.Where(c =>
                string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase));

            foreach (var claim in userPermissionClaims ?? Enumerable.Empty<Claim>())
            {
                var userPermision = claim.Value?.Split(':');

                if (userPermision == null || userPermision.Length != 3)
                {
                    continue;
                }

                var match = expectedRequirements
                    .Where(r =>
                        r.Item1 == userPermision[0]
                        && (r.Item2 == "*" || userPermision[1] == "*" || r.Item2 == userPermision[1])
                        && (r.Item3 == "*" || userPermision[2] == "*" || r.Item3 == userPermision[2]));

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
