using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalkingDinner.Extensions {

    public static class DateTimeExtensions {

        public static DateTime SetTime( this DateTime dateTime, int hours, int minutes, int seconds = 0, int milliseconds = 0 ) {

            return new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds, milliseconds, dateTime.Kind );
        }
    }
}
