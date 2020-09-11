using System;

namespace CalendarAPI
{
    public class Timer : CalendarElement
    {
        public bool keepTrack;         // should this timer be displayed continually until occurrence
        public int pausedTime;

        public Timer(dynamic timerJson)
        {
            //Date = timerJson["Date"];
            Date = $"{timerJson["month"]},{timerJson["day"]},{timerJson["year"]}";
            Content = timerJson["Content"];
            keepTrack = timerJson["keepTrack"];
            pausedTime = timerJson["pausedTime"];
        }

        public Timer(string dateString, bool track, string msg)
        {
            Date = dateString;
            keepTrack = track;
            Content = msg;
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
                    displayString = "\u2022 (TIMER) " + Content + " (in " + daysTill + " days)";
                else
                    displayString = "\u2022 (TIMER) " + Content + " (in " + daysTill + " day)";
            }
            else
            {
                if (daysTill > 1)
                    displayString = "\u2022 (TIMER)(PAUSED) " + Content + " (in " + daysTill + " days)";
                else
                    displayString = "\u2022 (TIMER)(PAUSED) " + Content + " (in " + daysTill + " day)";
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
