using Appointment.DataAccess.MSSQL;
using Appointment.Domain.MainRepo;
using Appointment.Domain.User.UserAuth;
using Appointment.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Domain.DependencyInjection
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
