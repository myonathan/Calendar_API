using Appointment.Common.Constants;
using Appointment.Domain.Model;
using Appointment.Resources;
using Appointment.Service;
using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appointment.Grpc
{
    public class AppointmentService : Appointment.AppointmentBase
    {
        private readonly ILogger<AppointmentService> _logger;
        private readonly IAppointmentService _appointmentService;

        public AppointmentService(ILogger<AppointmentService> logger, IAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
        }

        public override async Task<ListAppointmentResponse> GetAppointments(GetAppointmentRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _appointmentService.GetAppointments(request.StartDate.ToDateTime(), request.EndDate.ToDateTime());

                if (result == null)
                    throw new ExpectedException(nameof(Constants.CommonMessages.ER001), Constants.CommonMessages.ER001);

                var mapList = TinyMapper.Map<List<AppointmentModel>, List<AppointmentResponse>>(result);
                var returning = new ListAppointmentResponse();
                returning.Appointments.AddRange(mapList);

                return await Task.FromResult(returning);
            }
            catch (Exception e)
            {
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }
    }
}
