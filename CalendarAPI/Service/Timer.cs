using System;

namespace CalendarAPI
{
    public class Timer : CalendarElement
    {
        public bool keepTrack;         // should this timer be displayed continually until occurrence
        public int pausedTime;

        public Timer(dynamic timerJson) : base((uint)timerJson["ID"])
        {
            //Date = timerJson["Date"];
            Date = $"{timerJson["month"]},{timerJson["day"]},{timerJson["year"]}";
            Content = timerJson["Content"];
            keepTrack = timerJson["keepTrack"];
            pausedTime = timerJson["pausedTime"];
        }

        public Timer(uint id, string dateString, bool track, string msg) : base(id)
        {
            Date = dateString;
            keepTrack = track;
            Content = msg;
            pausedTime = 0;
        }


        /// <summary>
        /// Sets the displaystring to be used in the listbox
        /// * (TAG) (CONTENT) (YEARS TILL/AGO or none if happened this date)
        /// </summary>
        /// <param name="daysTill">Years ago, till, or none, appended to (TAG) (CONTENT) </param>
        public string DisplayString(int daysTill)
        {
            if (pausedTime == 0)
            {
                if (daysTill > 1)
                    return " (TIMER) " + Content + " (in " + daysTill + " days)";
                else
                    return " (TIMER) " + Content + " (in " + daysTill + " day)";
            }
            else
            {
                if (daysTill > 1)
                    return " (TIMER)(PAUSED) " + Content + " (in " + daysTill + " days)";
                else
                    return " (TIMER)(PAUSED) " + Content + " (in " + daysTill + " day)";
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
                Date = currentCalendar.dateIn(pausedTime);

        }

        public void TogglePause(HarptosCalendar currentCalendar)
        {
            if (pausedTime == 0)
                Pause(currentCalendar);
            else
                Unpause();
        }

        public void Pause(HarptosCalendar currentCalendar)
        {
            if (pausedTime == 0)
                pausedTime = currentCalendar.daysTo(Date);
        }

        public void Unpause()
        {
            pausedTime = 0;
        }
    }
}
