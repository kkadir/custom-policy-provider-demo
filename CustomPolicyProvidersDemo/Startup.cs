using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using CustomPolicyProvidersDemo.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Logging;

namespace CustomPolicyProvidersDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();

            services.AddSingleton<IAuthorizationHandler, PermissionsAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ScopesAuthorizationHandler>();

            services.AddControllers();
            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>("Bearer", null);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
