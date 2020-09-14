using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarAPI.Models
{
    /// <summary>
    /// 
    /// 
    /// </summary>

    public class CalendarDataModel
    {
        public CalendarElement [] CurrentDayElements { get; set; }
        public CalendarElement[] CurrentMonthElements { get; set; }

    }
}
