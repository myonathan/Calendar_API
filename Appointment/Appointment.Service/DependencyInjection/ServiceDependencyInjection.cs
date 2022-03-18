using Appointment.DataAccess.MSSQL;
using Appointment.Domain.MainRepo;
using Koobits.Domain.KoobitsUser.UserAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Service.DependencyInjection
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServiceExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LyteVenture_CalendarContext>(options => options.UseSqlServer(configuration.GetConnectionString("LyteVenture_CalendarContext")));
            services.AddTransient<IAppointmentMainRepo, AppointmentMainRepo>();
            services.AddTransient<IAppointmentService, AppointmentService>();
            return services;
        }
    }
}
