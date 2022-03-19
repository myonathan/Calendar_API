using Appointment.Domain.Model;
using AppointmentClient;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Nelibur.ObjectMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Appointment.Domain.ConverterMapperConfigs;
using System.ComponentModel;
using Appointment.GrpcClient.Converters;

namespace Appointment.GrpcClient
{
    class Program
    {
        private static void InitiateMapper()
        {
            #region mapper
            AppointmentMapperConfig.InitateMapper();
            // extra
            TypeDescriptor.AddAttributes(typeof(AppointmentModel), new TypeConverterAttribute(typeof(ToAppoinmentResponse)));
            TinyMapper.Bind<AppointmentModel, AppointmentResponse>();

            TypeDescriptor.AddAttributes(typeof(AppointmentResponse), new TypeConverterAttribute(typeof(ResponseToModel)));
            TinyMapper.Bind<AppointmentResponse, AppointmentModel>();

            TypeDescriptor.AddAttributes(typeof(AppointmentModel), new TypeConverterAttribute(typeof(ToAppoinmentRequest)));
            TinyMapper.Bind<AppointmentModel, AppointmentRequest>();
            TinyMapper.Bind<AppointmentRequest, AppointmentModel>();

            TypeDescriptor.AddAttributes(typeof(List<AppointmentModel>), new TypeConverterAttribute(typeof(ToAppointmentListResponse)));
            TinyMapper.Bind<List<AppointmentModel>, List<AppointmentResponse>>();

            TypeDescriptor.AddAttributes(typeof(List<AppointmentResponse>), new TypeConverterAttribute(typeof(ToAppointmentListModel)));
            TinyMapper.Bind<List<AppointmentResponse>, List<AppointmentModel>>();

            TypeDescriptor.AddAttributes(typeof(AppointmentRequest), new TypeConverterAttribute(typeof(ToAppointmentModel)));
            TinyMapper.Bind<AppointmentRequest, AppointmentModel>();
            #endregion
        }

        static async Task Main(string[] args)
        {
            InitiateMapper();

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new AppointmentClient.Appointment.AppointmentClient(channel);
            var request = new GetAppointmentRequest();

            var addApp = client.AddAppointment(new AppointmentRequest
            {
                Name = $"Test Console {new Random().Next(0, int.MaxValue)}",
                StartDate = DateTime.Now.ToUniversalTime().ToTimestamp(),
                EndDate = DateTime.Now.AddMinutes(1).ToUniversalTime().ToTimestamp()
            }
            );

            // UTC then convert back 
            request.StartDate = new DateTime(2022, 3, 1).ToUniversalTime().ToTimestamp();
            request.EndDate = new DateTime(2022, 3, 31).ToUniversalTime().ToTimestamp();

            var result = client.GetAppointments(request);

            var resultModels = TinyMapper.Map<List<AppointmentResponse>, List<AppointmentModel>>(result.Appointments.ToList());

            foreach (var x in resultModels)
                Console.WriteLine($"Name: {x.Name} Start: {x.StartDate.ToString("o")} End: {x.EndDate.ToString("o")}");

            var updateApp = TinyMapper.Map<AppointmentResponse, AppointmentModel>(addApp);
            var updateReq = TinyMapper.Map<AppointmentModel, AppointmentRequest>(updateApp);

            var updateResp = client.UpdateAppointment(updateReq);

            var appModelToDelete = TinyMapper.Map<AppointmentResponse, AppointmentModel>(updateResp);
            var deleteReq = TinyMapper.Map<AppointmentModel, AppointmentRequest>(appModelToDelete);

            var deleteResponse = client.DeleteAppointment(deleteReq);

            Console.WriteLine($"Deleted?: {deleteResponse.Result}");
            Console.ReadLine();
        }
    }
}
