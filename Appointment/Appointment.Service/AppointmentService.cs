using Koobits.Domain.KoobitsUser.UserAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentMainRepo _appointmentMainRepo;

        public AppointmentService(IAppointmentMainRepo appointmentMainRepo)
        {
            _appointmentMainRepo = appointmentMainRepo;
        }
    }
}
