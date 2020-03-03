using System;
using Microsoft.AspNetCore.Authorization;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class ScopesRequirement : IAuthorizationRequirement, IIdentifiable
    {
        public ScopesRequirement(string scopes, Guid identifier)
        {
            Scopes = scopes;
            Identifier = identifier;
        }

        public string Scopes { get; }

        public Guid Identifier { get; set; }
    }
}
