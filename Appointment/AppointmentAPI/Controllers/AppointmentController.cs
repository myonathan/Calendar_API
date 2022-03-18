using Appointment.Domain.Model;
using Appointment.Service;
using Appointment.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppointmentAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppointmentController : ExtendedApiController
    {
        public IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [Route("GetAppointments")]
        public ServiceResponse<List<AppointmentModel>> GetAppointments()
        {
           return Run(() =>
           {
               return new List<AppointmentModel>();
           });
        }

        [HttpPost]
        [Route("AddAppointment")]
        public ServiceResponse<AppointmentModel> AddAppointmentt([FromBody] AppointmentModel model)
        {
            return Run(() =>
            {
                return new AppointmentModel();
            });
        }

        [HttpPut]
        [Route("UpdateAppointment")]
        public ServiceResponse<AppointmentModel> UpdateAppointmentt([FromBody] AppointmentModel model)
        {
            return Run(() =>
            {
                return new AppointmentModel();
            });
        }

        [HttpPut]
        [Route("DeleteAppointment")]
        public ServiceResponse<AppointmentModel> DeleteAppointment([FromBody] AppointmentModel model)
        {
            return Run(() =>
            {
                return new AppointmentModel();
            });
        }
    }
}
