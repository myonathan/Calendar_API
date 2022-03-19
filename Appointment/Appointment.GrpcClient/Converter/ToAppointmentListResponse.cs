using Appointment.Domain.Model;
using AppointmentClient;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Type = System.Type;

namespace Appointment.GrpcClient.Converters
{
    public class ToAppointmentListResponse : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(List<AppointmentResponse>);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var concreteValue = (List<AppointmentModel>)value;

            var result = new List<AppointmentResponse>();

            foreach (var val in concreteValue)
            {
                result.Add(new AppointmentResponse
                {
                    Id = val.Id,
                    Name = val.Name,
                    Description = val.Description,
                    Location = val.Location,
                    Url = val.Url,
                    StartDate = val.StartDate.ToUniversalTime().ToTimestamp(),
                    EndDate = val.EndDate.ToUniversalTime().ToTimestamp()
                });
            }
            return result;
        }
    }
}
