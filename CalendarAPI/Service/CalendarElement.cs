﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarAPI.Service
{
    public abstract class CalendarElement
    {
        string content;
        Campaign campaign;
        string date;

        public string Content { get; set; }

        public Campaign Campaign { get; set; }

        public string Date { 
            get {
                return date;
            }
            set
            {
                if (VerifyDate(value))
                    date = value;
                else
                    return;
            }
        }



        public static bool VerifyDate(string dateString)
        {

            if (dateString == null)
                return false;
            var splitString = dateString.Split(",");

            int result;                                                                           // Date is bad...
            if (Int32.TryParse(splitString[0], out result) == false || result > 12)   // if month above 12
                return false;

            if (Int32.TryParse(splitString[1], out result) == false || result > 32)   // if day above 32
                return false;

            if (Int32.TryParse(splitString[2], out result) == false || result > 1600) // if year above 1600
                return false;

            return true; // If you reach this point, date must be good

            // Note this function could return a false date as true, a 32 day month only happens on leap year

        }

        // Returns 1 if x happened after y, -1 if y happened after x, 0 if same date
        public static int compareDate(CalendarElement x, CalendarElement y)
        {
            return HarptosCalendar.FarthestInTime(x.Date, y.Date);
        }
    }
}
