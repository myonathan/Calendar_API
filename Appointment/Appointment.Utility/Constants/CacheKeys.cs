using System;

namespace Appointment.Common.Constants
{
    public static class CacheKeys
    {
        private static readonly TimeSpan TimeSpan5Minutes  = new TimeSpan(0, 5, 0);

        public static class Appointment
        {
            public static string StaticAppointmentStartEndDate = "Appointment:{0}:{1}";
            public static TimeSpan AppointmentTimespan = TimeSpan5Minutes;
        }
    }
}