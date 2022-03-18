using Appointment.DataAccess.MSSQL;
using Appointment.Domain.MainRepo;
using Appointment.Domain.User.UserAuth;
using Appointment.Service;
using Castle.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Appointment.UnitTesting.ServiceTest
{
    public class AppointmentServiceTest
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentServiceTest()
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<LyteVenture_CalendarContext>(options => options.UseSqlServer("Server=localhost\\localdb;Database=LyteVenture_Calendar;integrated security=False;user id=lyteventure;password=qwerty12345"));
                    services.AddTransient<IAppointmentMainRepo, AppointmentMainRepo>();
                    services.AddTransient<IAppointmentService, AppointmentService>();
                    var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", optional: false);
                    var config = builder.Build();
                    services.AddTransient<Microsoft.Extensions.Configuration.IConfiguration>(x => config);

                    var mock = new Mock<ILogger<IAppointmentService>>();
                    var logger = mock.Object;

                    services.AddTransient<ILogger<IAppointmentService>>(x => logger);

                    var redisConfiguration = config.GetSection("Redis").Get<RedisConfiguration>();
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.ConfigurationOptions = redisConfiguration.ConfigurationOptions;
                    });

                    services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfiguration);

                }).UseConsoleLifetime();

            var host = builder.Build();
            var serviceScope = host.Services.CreateScope();
            var services = serviceScope.ServiceProvider;
            _appointmentService = services.GetRequiredService<IAppointmentService>();
        }

        [Fact]
        public void TestServiceNotNull()
        {
            Assert.NotNull(_appointmentService);
        }
    }
}