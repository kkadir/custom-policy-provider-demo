using Microsoft.AspNetCore.Authorization;

namespace CustomPolicyProvidersDemo.Authorization
{
    public class ScopesRequirement : IAuthorizationRequirement
    {
        public ScopesRequirement(string scopes)
        {
            Scopes = scopes;
        }

        public string Scopes { get; }
    }
}
