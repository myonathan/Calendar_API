using Appointment.DataAccess.Core.Repository;
using Appointment.DataAccess.MSSQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Domain.MainRepo
{
    public class AppointmentMainRepo : AppointmentGenericRepository<DataAccess.Entity.Appointment>
    {
        public AppointmentMainRepo(LyteVenture_CalendarContext context)
              : base(context)
        {

        }
    }
}
