using Microsoft.AspNetCore.Authorization;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class RolesRequirement : IAuthorizationRequirement
    {
        public RolesRequirement(string roles)
        {
            Roles = roles;
        }

        public string Roles { get; }
    }
}
