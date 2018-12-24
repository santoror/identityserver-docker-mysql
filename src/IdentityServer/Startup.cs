using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString,
                        sql =>
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                            sql.ServerVersion(new Version(8, 0, 13), ServerType.MySql);
                            sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), new List<int>() { 1, 2, 3 });
                        }));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();


            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseMySql(connectionString,
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.ServerVersion(new Version(8, 0, 13), ServerType.MySql);
                                sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), new List<int>() { 1, 2, 3 });
                            });
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseMySql(connectionString,
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                                sql.ServerVersion(new Version(8, 0, 13), ServerType.MySql);
                                sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), new List<int>() { 1, 2, 3 });
                            });

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                });

            //var cert = new X509Certificate2(File.ReadAllBytes(Configuration["Certificate:Path"]), Configuration["Certificate:Password"]);
            //builder.AddSigningCredential(cert);

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }

            services.AddAuthentication()
                .AddOpenIdConnect("oidc", "OpenID Connect Identity Server", options =>
                {
                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "implicit";
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //app.UseHsts();
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
