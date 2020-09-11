using System;

namespace CalendarAPI
{
    public class Timer
    {
        public int month;              // Month timer occurs;
        public int day;                // day timer occurs;
        public int year;               // year timer occurs;
        public bool keepTrack;         // should this timer be displayed continually until occurrence
        public string message;  // What the timer shows when it occurs
        public int pausedTime;

        [Newtonsoft.Json.JsonConstructor]
        public Timer(int m, int d, int y, bool track, string msg)
        {
            month = m;
            day = d;
            year = y;
            keepTrack = track;
            message = msg;
            pausedTime = 0;
        }

        public Timer(dynamic timerJson)
        {
            month = timerJson["month"];
            day = timerJson["day"];
            year = timerJson["year"];
            keepTrack = timerJson["keepTrack"];
            message = timerJson["message"];
            pausedTime = timerJson["pausedTime"];
        }

        public Timer(string dateString, bool track, string msg)
        {
            month = Int32.Parse(dateString.Substring(0, 2));
            day = Int32.Parse(dateString.Substring(2, 2));
            year = Int32.Parse(dateString.Substring(4, 4));
            keepTrack = track;
            message = msg;
            pausedTime = 0;
        }

        string displayString;
        public string DisplayString
        {
            get
            {
                return displayString;
            }
        }

        /// <summary>
        /// Sets the displaystring to be used in the listbox
        /// * (TAG) (CONTENT) (YEARS TILL/AGO or none if happened this date)
        /// </summary>
        /// <param name="daysTill">Years ago, till, or none, appended to (TAG) (CONTENT) </param>
        public void SetDisplayString(int daysTill)
        {
            if (pausedTime == 0)
            {
                if (daysTill > 1)
                    displayString = "\u2022 (TIMER) " + message + " (in " + daysTill + " days)";
                else
                    displayString = "\u2022 (TIMER) " + message + " (in " + daysTill + " day)";
            }
            else
            {
                if (daysTill > 1)
                    displayString = "\u2022 (TIMER)(PAUSED) " + message + " (in " + daysTill + " days)";
                else
                    displayString = "\u2022 (TIMER)(PAUSED) " + message + " (in " + daysTill + " day)";
            }
        }

        /// <summary>
        /// If a timer is paused, when days are incremented, the timer's alarm date should also be incremented to reflect the pause
        /// </summary>
        /// <param name="currentCalendar"></param>
        public void AdjustForPause(HarptosCalendar currentCalendar)
        {
            if (pausedTime == 0)
                return;
            else
            {
                string newDate;
                newDate = currentCalendar.dateIn(pausedTime);

                month = Int32.Parse(newDate.Substring(0, 2));
                day = Int32.Parse(newDate.Substring(2, 2));
                year = Int32.Parse(newDate.Substring(4, 4));
            }

        }

        public void TogglePause(HarptosCalendar currentCalendar)
        {
            if (pausedTime == 0)
                Pause(currentCalendar);
            else
                Unpause(currentCalendar);
        }

        public void Pause(HarptosCalendar currentCalendar)
        {
            if (pausedTime == 0)
            {
                pausedTime = currentCalendar.daysTo(month, day, year);
            }
        }

        public void Unpause(HarptosCalendar currentCalendar)
        {
            pausedTime = 0;
        }


        public string returnDateString()
        {
            string monthString = HarptosCalendar.enforceMonthFormat(month.ToString());
            string yearString = HarptosCalendar.enforceYearFormat(year.ToString());
            string dayString = HarptosCalendar.enforceDayFormat(monthString, day.ToString(), yearString);
            return monthString + dayString + yearString;
        }

    }
}
