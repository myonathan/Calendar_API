using Appointment.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Service
{
    public interface IAppointmentService
    {
        public Task<List<AppointmentModel>> GetAppointments(DateTime start, DateTime end);
        public bool DeleteAppointment(AppointmentModel appointment);
        public AppointmentModel AddAppointment(AppointmentModel appointment);
        public AppointmentModel UpdateAppointment(AppointmentModel appointment);
    }
}
