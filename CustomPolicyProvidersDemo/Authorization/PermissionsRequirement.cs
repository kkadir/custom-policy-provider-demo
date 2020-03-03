using Microsoft.AspNetCore.Authorization;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class PermissionsRequirement : IAuthorizationRequirement
    {
        public PermissionsRequirement(string permissions)
        {
            Permissions = permissions;
        }

        public string Permissions { get; }
    }
}
