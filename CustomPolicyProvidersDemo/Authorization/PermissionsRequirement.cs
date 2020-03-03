using System;
using Microsoft.AspNetCore.Authorization;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class PermissionsRequirement : IAuthorizationRequirement, IIdentifiable
    {
        public PermissionsRequirement(string permissions, Guid identifier)
        {
            Permissions = permissions;
            Identifier = identifier;
        }

        public string Permissions { get; }

        public Guid Identifier { get; set; }
    }
}
