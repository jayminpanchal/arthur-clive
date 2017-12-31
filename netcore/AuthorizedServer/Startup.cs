using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AuthorizedServer.Helper;
using AuthorizedServer.Models;
using AuthorizedServer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using MH = AuthorizedServer.Helper.MongoHelper;
namespace AuthorizedServer
{
    /// <summary></summary>
    public class Startup
    {
        /// <summary></summary>
        public List<string> level1RoleList = new List<string>();
        /// <summary></summary>
        public List<string> level2RoleList = new List<string>();
        /// <summary></summary>
        public List<string> level3RoleList = new List<string>();

        /// <summary></summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            if (env.IsEnvironment("Development"))
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary></summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary></summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region JWT
            services.AddSingleton<IRTokenRepository, RTokenRepository>();
            #endregion
            services.AddMvc();
            #region Cors
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            #endregion
            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "AuthorizedServer",
                    Version = "v1",
                    Description = "Controller methods for Authentication and Generation of JWT Token",
                    TermsOfService = "None",
                    Contact = null,
                    License = null
                });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "AuthorizedServer.xml");
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<ExamplesOperationFilter>();
            });
            #endregion
            services.AddOptions();
            services.Configure<Audience>(Configuration.GetSection("Audience"));

            #region Role based authorization
            CreatePolicy();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Level1Access", policy => policy.RequireRole(level1RoleList));
                options.AddPolicy("Level2Access", policy => policy.RequireRole(level2RoleList));
                options.AddPolicy("Level3Access", policy => policy.RequireRole(level3RoleList));
            });
            #endregion
        }

        /// <summary></summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="serviceProvider"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseStaticFiles();
            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arthur_Clive");
            });
            #endregion
            #region Cors
            app.UseCors("CorsPolicy");
            #endregion
            app.UseMvc();            
        }

        /// <summary>Add roles to policy list based on access level</summary>
        public void CreatePolicy()
        {
            var roles = MH.GetListOfObjects(null, null, null, null, null, null, "RolesDB", "Roles").Result;
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var data = BsonSerializer.Deserialize<Roles>(role).LevelOfAccess;
                    foreach (var access in data)
                    {
                        if (access == "Level1Access")
                        {
                            level1RoleList.Add((BsonSerializer.Deserialize<Roles>(role).RoleName));
                        }
                        else if (access == "Level2Access")
                        {
                            level2RoleList.Add((BsonSerializer.Deserialize<Roles>(role).RoleName));
                        }
                        else if (access == "Level3Access")
                        {
                            level3RoleList.Add((BsonSerializer.Deserialize<Roles>(role).RoleName));
                        }
                    }
                }
            }
        }
    }
}
