using Appointment.DataAccess.MSSQL;
using Appointment.Domain.ConverterMapperConfigs;
using Appointment.Domain.MainRepo;
using Appointment.Domain.Model;
using Appointment.Domain.User.UserAuth;
using Appointment.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Linq;
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

            AppointmentMapperConfig.InitateMapper();
        }

        [Fact]
        public void TestServiceNotNull()
        {
            Assert.NotNull(_appointmentService);
        }

        [Fact]
        public void AddAppointment()
        {
            var rand = new Random();
            var randInt = rand.Next(1, int.MaxValue);

            var start = DateTime.Now;
            var end = start.AddMinutes(randInt);

            var result1 = _appointmentService.AddAppointment(new AppointmentModel
            {
                Name = $"Test Appointment {randInt}",
                Description = $"Description {randInt}",
                Location = "Location 1",
                Url = "https://test.com",
                StartDate = start,
                EndDate = end
            });

            Assert.True(result1.Id > 0);

            randInt = rand.Next(1, int.MaxValue);

            try
            {
                _appointmentService.AddAppointment(new AppointmentModel
                {
                    Name = $"Test Appointment {randInt}",
                    Description = $"Description {randInt}",
                    Location = "Location 1",
                    Url = "https://test.com",
                    StartDate = start.AddMinutes(1),
                    EndDate = end
                });
            }
            catch (Exception ex)
            {
                Assert.Contains("New appointment conflict with another appointments", ex.Message);
            }

            start = end.AddMinutes(1);
            end = start.AddMinutes(randInt + 1);

            randInt = rand.Next(1, int.MaxValue);

            var result2 = _appointmentService.AddAppointment(new AppointmentModel
            {
                Name = $"Test Appointment {randInt}",
                Description = $"Description {randInt}",
                Location = "Location 1",
                Url = "https://test.com",
                StartDate = start,
                EndDate = end
            });

            Assert.True(result2.Id > 0);
            Assert.True(_appointmentService.DeleteAppointment(result1));
            Assert.True(_appointmentService.DeleteAppointment(result2));
        }

        [Fact]
        public void UpdateAppointment()
        {
            var rand = new Random();
            var randInt = rand.Next(1, int.MaxValue);

            var start = DateTime.Now;
            var end = start.AddMinutes(randInt);

            var result = _appointmentService.AddAppointment(new AppointmentModel
            {
                Name = $"Test Appointment {randInt}",
                Description = $"Description {randInt}",
                Location = "Location 1",
                Url = "https://test.com",
                StartDate = start,
                EndDate = end
            });

            result.Name = $"Changed Test Appointment {randInt}";

            result = _appointmentService.UpdateAppointment(result);

            Assert.Contains($"Changed Test Appointment {randInt}", result.Name);
            Assert.True(_appointmentService.DeleteAppointment(result));
        }

        [Fact]
        public void DeleteAppointment()
        {
            var rand = new Random();
            var randInt = rand.Next(1, int.MaxValue);

            var start = DateTime.Now;
            var end = start.AddMinutes(randInt);

            var result = _appointmentService.AddAppointment(new AppointmentModel
            {
                Name = $"Test Appointment {randInt}",
                Description = $"Description {randInt}",
                Location = "Location 1",
                Url = "https://test.com",
                StartDate = start,
                EndDate = end
            });


            Assert.True(_appointmentService.DeleteAppointment(result));
        }

        //Note: Make sure Redis server is running
        [Fact]
        public async void GetAppointments()
        {
            var rand = new Random();
            var randInt = rand.Next(1, int.MaxValue);

            var start = DateTime.Now;
            var end = start.AddMinutes(randInt);

            var result = _appointmentService.AddAppointment(new AppointmentModel
            {
                Name = $"Test Appointment {randInt}",
                Description = $"Description {randInt}",
                Location = "Location 1",
                Url = "https://test.com",
                StartDate = start,
                EndDate = end
            });

            var list = await _appointmentService.GetAppointments(start, end);
            Assert.True(list.Count() > 0);
            Assert.True(_appointmentService.DeleteAppointment(result));
        }
    }
}