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
    public class ToAppoinmentResponse : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AppointmentResponse);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var concreteValue = (AppointmentModel)value;

            return new AppointmentResponse
            {
                Id = concreteValue.Id,
                Name = concreteValue.Name,
                Description = concreteValue.Description,
                Location = concreteValue.Location,
                Url = concreteValue.Url,
                StartDate = concreteValue.StartDate.ToUniversalTime().ToTimestamp(),
                EndDate = concreteValue.EndDate.ToUniversalTime().ToTimestamp()
            };
        }
    }
}
