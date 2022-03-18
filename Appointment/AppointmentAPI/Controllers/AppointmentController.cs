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
        [Route("GetAppointments/{startDate}/{endDate}")]
        public async Task<ServiceResponse<List<AppointmentModel>>> GetAppointments(DateTime startDate, DateTime endDate)
        {
           return await Run(async () =>
           {
               return await _appointmentService.GetAppointments(startDate, endDate);
           });
        }

        [HttpPost]
        [Route("AddAppointment")]
        public ServiceResponse<AppointmentModel> AddAppointmentt([FromBody] AppointmentModel model)
        {
            return Run(() =>
            {
                return  _appointmentService.AddAppointment(model);
            });
        }

        [HttpPut]
        [Route("UpdateAppointment")]
        public ServiceResponse<AppointmentModel> UpdateAppointmentt([FromBody] AppointmentModel model)
        {
            return Run(() =>
            {
                return _appointmentService.UpdateAppointment(model);
            });
        }

        [HttpDelete]
        [Route("DeleteAppointment")]
        public ServiceResponse DeleteAppointment([FromBody] AppointmentModel model)
        {
            return Run(() =>
            {
                return _appointmentService.DeleteAppointment(model);
            });
        }
    }
}
