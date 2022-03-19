using Appointment.Domain.Model;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Type = System.Type;

namespace Appointment.Grpc.Converters
{
    public class ToAppointmentListModel : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(List<AppointmentModel>);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var concreteValue = (List<AppointmentResponse>)value;

            var result = new List<AppointmentModel>();

            foreach (var val in concreteValue)
            {
                result.Add(new AppointmentModel
                {
                    Id = val.Id,
                    Name = val.Name,
                    Description = val.Description,
                    Location = val.Location,
                    Url = val.Url,
                    StartDate = val.StartDate.ToDateTime().ToLocalTime(),
                    EndDate = val.EndDate.ToDateTime().ToLocalTime()
                });
            }
            return result;
        }
    }
}
