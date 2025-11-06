using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.EmploymentCheck.Api.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.EmploymentCheck.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            builder.AddJsonFile("appsettings.Development.json", optional: true);

            builder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"]?.Split(",") ?? Array.Empty<string>();
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
            services.AddNLogForApi();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SFA.DAS.EmploymentCheck.Api", Version = "v1.0" });
                c.CustomSchemaIds(type => type.FullName);
            });

            services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), Configuration));

            services.Configure<EmploymentCheckSettings>(Configuration.GetSection("ApplicationSettings"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<EmploymentCheckSettings>>().Value);

            var envName = Configuration["EnvironmentName"] ?? string.Empty;

            if (!envName.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
            {
                var aadTenantForAuthority = Configuration["AzureAd:Tenant"] ?? string.Empty;
                var identifier = Configuration["AzureAd:Identifier"];
                var clientId = Configuration["AzureAd:ClientId"];

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = $"https://login.microsoftonline.com/{aadTenantForAuthority}/v2.0";

                    var audiences = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    if (!string.IsNullOrWhiteSpace(identifier))
                    {
                        audiences.Add(identifier);

                        if (identifier.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                            && !identifier.EndsWith("-ar", StringComparison.OrdinalIgnoreCase))
                        {
                            audiences.Add($"{identifier}-ar");
                        }
                    }

                    string derivedEiAudience = TryDeriveEiAudienceFromIdentifier(identifier);
                    if (!string.IsNullOrWhiteSpace(derivedEiAudience))
                    {
                        audiences.Add(derivedEiAudience);
                    }

                    if (!string.IsNullOrWhiteSpace(clientId))
                    {
                        audiences.Add($"api://{clientId}");
                        audiences.Add(clientId);
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudiences = audiences,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(2)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                            logger.LogError(ctx.Exception,
                                "JWT auth failed. Authority={Authority}; Expected audiences=[{Audiences}]",
                                options.Authority, string.Join(", ", audiences));
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = ctx =>
                        {
                            var audClaims = ctx.Principal?.Claims.Where(c => c.Type == "aud").Select(c => c.Value).ToArray() ?? Array.Empty<string>();
                            var appId = ctx.Principal?.FindFirst("appid")?.Value ?? ctx.Principal?.FindFirst("azp")?.Value ?? "(none)";
                            var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                            logger.LogInformation("Token validated. aud=[{Aud}], appid/azp={AppId}", string.Join(",", audClaims), appId);
                            return Task.CompletedTask;
                        }
                    };
                });

                services.AddAuthorization(o =>
                {
                    o.AddPolicy(PolicyNames.Default, p => p.RequireAuthenticatedUser());
                });

                services.AddMvc(o =>
                {
                    o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            }

            services
                .AddRepositories()
                .AddServices()
                .AddHandlers();

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            });
        }

        private static string TryDeriveEiAudienceFromIdentifier(string identifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier) || !identifier.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    return null;

                var uri = new Uri(identifier);

                var candidate = uri.AbsolutePath.Trim('/');
                var segments = candidate.Split('/', StringSplitOptions.RemoveEmptyEntries);

                string match = segments.FirstOrDefault(s => Regex.IsMatch(s, @"^das-[a-z0-9\-]+-echkapi$", RegexOptions.IgnoreCase));
                if (match == null && Regex.IsMatch(candidate, @"^das-[a-z0-9\-]+-echkapi$", RegexOptions.IgnoreCase))
                {
                    match = candidate;
                }

                if (match == null)
                    return null;

                var asArSegment = match + "-as-ar";

                var rebuiltPath = string.Join("/",
                    segments.Select(s => s.Equals(match, StringComparison.OrdinalIgnoreCase) ? asArSegment : s));

                if (segments.Length == 0) rebuiltPath = asArSegment;

                var result = $"https://{uri.Host}/{rebuiltPath}";
                return result;
            }
            catch
            {
                return null;
            }
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
