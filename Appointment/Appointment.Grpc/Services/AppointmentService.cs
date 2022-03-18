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

        public override async Task<DeleteResponse> DeleteAppointment(AppointmentRequest request, ServerCallContext context)
        {
            try
            {
                var appointmentModel = TinyMapper.Map<AppointmentRequest, AppointmentModel>(request);
                var result = _appointmentService.DeleteAppointment(appointmentModel);
               
                var deleteResponse = new DeleteResponse();
                deleteResponse.Result = result;
                return await Task.FromResult(deleteResponse);
            }
            catch (Exception e)
            {
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }

        public override async Task<global::Appointment.Grpc.AppointmentResponse> AddAppointment(.AppointmentRequest request, ServerCallContext context)
        {
            try
            {
                var appointmentModel = TinyMapper.Map<AppointmentRequest, AppointmentModel>(request);

                var result = _appointmentService.AddAppointment(appointmentModel);

                return await Task.FromResult(TinyMapper.Map<AppointmentModel, AppointmentResponse>(result));
            }
            catch (Exception e)
            {
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }

        public override async Task<AppointmentResponse> UpdateAppointment(AppointmentRequest request, ServerCallContext context)
        {
            try
            {
                var appointmentModel = TinyMapper.Map<AppointmentRequest, AppointmentModel>(request);

                var result = _appointmentService.UpdateAppointment(appointmentModel);

                return await Task.FromResult(TinyMapper.Map<AppointmentModel, AppointmentResponse>(result));
            }
            catch (Exception e)
            {
                throw new RpcException(Status.DefaultCancelled, e.Message);
            }
        }
    }
}
