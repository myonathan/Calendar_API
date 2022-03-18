using System;
using System.Collections.Generic;
using System.Text;

namespace Appointment.Common.Constants
{
    public static class Constants
    {
        public static string Errors = "Errors";

        public static class CommonMessages
        {
            public const string ER001 = "No Data Found";
        }

        public static class Appointment
        {
            public const string APP01 = "Invalid Id {0}";
            public const string APP02 = "New appointment conflict with another appointments {0}";
            public const string APP03 = "Name cannot be empty";
            public const string APP04 = "Start date cannot be bigger than equal end date";
        }
    }
}
