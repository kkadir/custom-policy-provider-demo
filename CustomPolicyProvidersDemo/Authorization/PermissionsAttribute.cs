using System;
using Microsoft.AspNetCore.Authorization;

namespace CustomPolicyProvidersDemo.Authorization
{
    public sealed class PermissionsAttribute : AuthorizeAttribute
    {
        public const string PermissionsGroup = "Permissions";
        public const string RolesGroup = "Roles";
        public const string ScopesGroup = "Scopes";

        private string[] _permissions;
        private string[] _scopes;
        private string[] _roles;

        private bool _isDefault = true;

        public PermissionsAttribute()
        {
            _permissions = Array.Empty<string>();
            _roles = Array.Empty<string>();
            _scopes = Array.Empty<string>();
        }

        public string[] Permissions
        {
            get => _permissions;
            set
            {
                BuildPolicy(ref _permissions, value, PermissionsGroup);
            }
        }

        public string[] Scopes
        {
            get => _scopes;
            set
            {
                BuildPolicy(ref _scopes, value, ScopesGroup);
            }
        }

        new public string[] Roles
        {
            get => _roles;
            set
            {
                BuildPolicy(ref _roles, value, RolesGroup);
            }
        }

        private void BuildPolicy(ref string[] target, string[] value, string group)
        {
            target = value ?? Array.Empty<string>();

            if (_isDefault)
            {
                Policy = string.Empty;
                _isDefault = false;
            }

            Policy += $"{group}${string.Join("|", target)};";
        }
    }
}
