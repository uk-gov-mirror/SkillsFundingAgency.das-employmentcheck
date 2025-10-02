using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Collections.Generic;

namespace SFA.DAS.EmploymentCheck.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddEnvironmentVariables();

            builder.AddJsonFile("appsettings.Development.json", optional: true);

            builder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"]?.Split(",") ?? new string[0];
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHealthChecks();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SFA.DAS.EmploymentCheck.Api", Version = "v1.0" });
                c.CustomSchemaIds(type => type.FullName);
            });

            var envName = Configuration["EnvironmentName"] ?? "";

            if (!envName.Equals("LOCAL", System.StringComparison.OrdinalIgnoreCase))
            {
                var azureAd = Configuration.GetSection("AzureAd").Get<AzureActiveDirectoryConfiguration>() ?? new AzureActiveDirectoryConfiguration();
                var tenant = Configuration["AzureAd:Tenant"] ?? azureAd.Tenant ?? "";
                var identifierUri = Configuration["AzureAd:IdentifierUri"];
                var clientId = Configuration["AzureAd:ClientId"];

                var policies = new Dictionary<string, string> { { "default", PolicyNames.Default } };
                services.AddAuthentication(azureAd, policies);

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = $"https://login.microsoftonline.com/{tenant}/v2.0";
                        var audiences = new List<string>();
                        if (!string.IsNullOrWhiteSpace(identifierUri)) audiences.Add(identifierUri);
                        if (!string.IsNullOrWhiteSpace(clientId)) audiences.Add($"api://{clientId}");
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = true,
                            ValidAudiences = audiences
                        };
                    });

                services.AddMvc(o =>
                {
                    o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            }

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/ping");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SFA.DAS.EmploymentCheck.Api v1.0"));
        }
    }
}
