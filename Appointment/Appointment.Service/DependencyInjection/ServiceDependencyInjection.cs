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
            return services;
        }
    }
}
