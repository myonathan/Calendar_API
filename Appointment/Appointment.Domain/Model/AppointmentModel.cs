using Appointment.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Appointment.Resources;

namespace Appointment.Domain.Model
{
    public class AppointmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public void Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(Name))
                errors.Add(nameof(Constants.Appointment.APP03) + " " + Constants.Appointment.APP03);

            if(StartDate >= EndDate)
                errors.Add(nameof(Constants.Appointment.APP04) + " " + Constants.Appointment.APP04);

            if (errors.Any())
                throw new ExpectedException(Constants.Errors, string.Join(System.Environment.NewLine, errors));
        }
    }
}
