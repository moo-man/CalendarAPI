using System;

namespace CalendarAPI
{
    public enum noteType { note, generalNote, timer, universal };

    public class Note : CalendarElement
    {
        public AlertScope Importance { get; set; } // who should be notified when this date is reached)
        public bool isGeneral { get { return Campaign == null; } }

        /// <summary>
        /// Sets the displaystring to be used in the listbox
        /// * (TAG) (CONTENT) (YEARS TILL/AGO or none if happened this date)
        /// </summary>
        /// <param name="relativity">Years ago, till, or none, appended to (TAG) (CONTENT) </param>
        public string DisplayString(string relativity)
        {
            if (campaign != null)
                return "(" + campaign.Tag + ") " + Content + " " + relativity;
            else
                return Content + " " + relativity;
        }

        public Note(uint id, string date, AlertScope imp, string content, Campaign campaign = null) : base(id)
        {
            Date = date;
            Importance = imp;
            Content = content;
            Campaign = campaign;
        }

        public Note(dynamic noteJson) : base ((uint)noteJson["ID"])
        {
            Date = noteJson["Date"];
            Content = noteJson["Content"];
            Importance = noteJson["Importance"];
        }


    }
}