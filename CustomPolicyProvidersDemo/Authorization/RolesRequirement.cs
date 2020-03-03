using System;
using Microsoft.AspNetCore.Authorization;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class RolesRequirement : IAuthorizationRequirement, IIdentifiable
    {
        public RolesRequirement(string roles, Guid identifier)
        {
            Roles = roles;
            Identifier = identifier;
        }

        public string Roles { get; }

        public Guid Identifier { get; set; }
    }
}
