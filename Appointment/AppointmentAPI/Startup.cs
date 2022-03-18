using AppointmentAPI.MiddleWare.RateLimitMiddleWare;
using AspNetCoreRateLimit;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.IdentityModel.Tokens.Jwt;
using Appointment.Domain.DependencyInjection;
using Microsoft.OpenApi.Models;
using Appointment.Domain.ConverterMapperConfigs;

namespace AppointmentAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region memory cache 
            services.AddMemoryCache();
            #endregion 

            services.AddControllers().AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.UseMemberCasing();
            }).AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddHttpContextAccessor();

            #region dependency injection

            services.AddServiceExtension(Configuration);
            #endregion

            #region redis 

            var redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = redisConfiguration.ConfigurationOptions;
            });

            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);
            #endregion

            #region jwt (not used)

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetSection("IdentityServer").GetSection("Authority").Value;
                    options.RequireHttpsMetadata = Convert.ToBoolean(Configuration.GetSection("IdentityServer")
                        .GetSection("RequireHttpsMetadata").Value);
                    options.ApiSecret = Configuration.GetSection("IdentityServer").GetSection("ApiSecret").Value;
                    options.ApiName = Configuration.GetSection("IdentityServer").GetSection("ApiName").Value;
                    // options.ClaimsIssuer=Configuration.GetSection("IdentityServer").GetSection("Authority").Value;
                    options.RoleClaimType = JwtClaimTypes.Role;
                    options.NameClaimType = JwtClaimTypes.Name;
                });

            #endregion 

            #region request limit, Note: This depends on redis 
            services.AddOptions();

            //  appsettings.json IpRateLimiting  
            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));

            //  appsettings.json  Ip Rule, for service invocation through client 
            //  example: https://github.com/stefanprodan/AspNetCoreRateLimit/blob/master/test/AspNetCoreRateLimit.Demo/Controllers/ClientRateLimitController.cs
            services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

            services.AddDistributedRateLimiting();
            services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, CustomRateLimitConfiguration>();
            #endregion

            #region swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AppointmentAPI",
                    Version = "v1"
                });
            });

            #endregion

            #region mapper
            AppointmentMapperConfig.InitateMapper();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;

                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration.GetValue<string>("VirtualDirectory") + "/swagger/v1/swagger.json",
                    "AppointmentAPI");
            });

            app.UseRouting();

            app.UseAuthorization();

            #region client rate limit 
            app.UseMiddleware<CustomUserClientRateLimitMiddleware>();
            #endregion 

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // https://chowdera.com/2021/07/20210704225021776h.html

            #region limit activation 
            app.UseClientRateLimiting();

            #endregion
        }
    }
}
