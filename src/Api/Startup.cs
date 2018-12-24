using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://identityserver.local";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api1";
                });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "Test Api", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = "http://identityserver.local/connect/authorize",
                    Scopes = new Dictionary<string, string>
                    {
                        {"demo_api", "Demo API - full access"}
                    }
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            //app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI(options =>
            //{
            //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //    options.OAuthClientId("demo_api_swagger");
            //    options.OAuthAppName("Demo API - Swagger");
            //});
            app.UseMvc();
        }
    }
}