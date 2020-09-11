using System;

namespace CalendarAPI
{
    public enum noteType { note, generalNote, timer, universal };

    public class Note : CalendarElement
    {
        public AlertScope Importance { get; set; } // who should be notified when this date is reached)
        public bool isGeneral { get { return Campaign == null; } }

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
        /// <param name="relativity">Years ago, till, or none, appended to (TAG) (CONTENT) </param>
        public void SetDisplayString(string relativity)
        {
            if (campaign != null)
                displayString = "\u2022 (" + campaign.Tag + ") " + Content + " " + relativity;
            else
                displayString = "\u2022 " + Content + " " + relativity;

        }

        public Note(string date, AlertScope imp, string content, Campaign campaign = null)
        {
            Date = date;
            Importance = imp;
            Content = content;
            Campaign = campaign;
        }

        public Note(dynamic noteJson)
        {
            Date = noteJson["Date"];
            Content = noteJson["Content"];
            Importance = noteJson["Importance"];
        }


    }
}