using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CustomPolicyProvidersDemo
{
    public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private ILogger<MockAuthenticationHandler> _logger;

        public MockAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger<MockAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var handler = new JwtSecurityTokenHandler();

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("private-key-to-sign-the-token"));

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "issuer",
                Audience = "audience",
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, "John Doe"),
                    new Claim("permission", "weather:forecast:read-data"),
                    new Claim("permission", "weather:*:filter-data"),
                    new Claim(ClaimTypes.Role, "user"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("scope", "App.Demo")
                })
            };

            var token = handler.CreateToken(securityTokenDescriptor);

            var tokenString = handler.WriteToken(token);

            var param = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = "audience",
                ValidateIssuer = true,
                ValidIssuer = "issuer",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };

            try
            {
                var claimsPrincipal = handler.ValidateToken(tokenString, param, out SecurityToken validatedToken);

                var authenticationProperties = new AuthenticationProperties();

                var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

                return AuthenticateResult.Success(ticket);

            }
            catch (Exception ex)
            {
                _logger.LogError("Authentication Failed.");

                return AuthenticateResult.Fail(ex);
            }
        }
    }
}