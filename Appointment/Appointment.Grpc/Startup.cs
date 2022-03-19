using Appointment.Domain.ConverterMapperConfigs;
using Appointment.Domain.DependencyInjection;
using Appointment.Domain.Model;
using Appointment.Grpc.Converters;
using Appointment.Infrastructure.MiddleWare.RateLimitMiddleWare;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nelibur.ObjectMapper;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Collections.Generic;
using System.ComponentModel;

namespace Appointment.Grpc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

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

            #region mapper
            AppointmentMapperConfig.InitateMapper();
            // extra
            TypeDescriptor.AddAttributes(typeof(AppointmentModel), new TypeConverterAttribute(typeof(ToAppoinmentResponse)));
            TinyMapper.Bind<AppointmentModel, AppointmentResponse>();
            TinyMapper.Bind<AppointmentResponse, AppointmentModel>();

            TypeDescriptor.AddAttributes(typeof(List<AppointmentModel>), new TypeConverterAttribute(typeof(ToAppointmentListResponse)));
            TinyMapper.Bind<List<AppointmentModel>, List<AppointmentResponse>>();

            TypeDescriptor.AddAttributes(typeof(List<AppointmentResponse>), new TypeConverterAttribute(typeof(ToAppointmentListModel)));
            TinyMapper.Bind<List<AppointmentResponse>, List<AppointmentModel>>();

            TypeDescriptor.AddAttributes(typeof(AppointmentRequest), new TypeConverterAttribute(typeof(ToAppointmentModel)));
            TinyMapper.Bind<AppointmentRequest, AppointmentModel>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AppointmentService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
