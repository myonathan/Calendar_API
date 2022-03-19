using AppointmentClient;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Appointment.GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new AppointmentClient.Appointment.AppointmentClient(channel);
            var request = new GetAppointmentRequest();
            request.StartDate = new DateTime(2022, 3, 1).ToTimestamp();
            request.EndDate = new DateTime(2022, 3, 31).ToTimestamp();

            var result = client.GetAppointments(request);
        }
    }
}
