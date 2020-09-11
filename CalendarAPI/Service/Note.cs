using System;

namespace CalendarAPI
{
    public enum noteType { note, generalNote, timer, universal };

    public class Note
    {
        string date;  //MMDDYYYY
        AlertScope importance; // who should be notified when this date is reached?
        string noteContent; // Note contents
        Campaign campaign;

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
                displayString = "\u2022 (" + campaign.Tag + ") " + NoteContent + " " + relativity;
            else
                displayString = "\u2022 " + NoteContent + " " + relativity;

        }

        public Note(string d, AlertScope imp, string n, Campaign c)
        {
            editDate(d);
            importance = imp;
            editNoteContent(n);
            campaign = c;
        }

        public Note(string d, AlertScope imp, string n)
        {
            date = d;
            importance = imp;
            noteContent = n;
        }

        public Note(dynamic noteJson)
        {
            Date = noteJson["Date"];
            NoteContent = noteJson["NoteContent"];
            Importance = noteJson["Importance"];
        }

        public string Date
        {
            get { return date; }
            set { editDate(value); }
        }

        public string NoteContent
        {
            get { return noteContent; }
            set { editNoteContent(value); }
        }

        public Campaign Campaign
        {
            get { return campaign; }
            set { campaign = value; }
        }

        public void editDate(string newDate)
        {
            if (VerifyDate(newDate))
                date = newDate;
            else
                date = "00,00,0000";
        }

        public AlertScope Importance
        {
            get { return importance; }
            set { importance = value; }
        }

        public bool isGeneral()
        {
            if (Campaign == null)
                return true;
            else
                return false;
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

        public void editNoteContent(string newNote)
        {
            noteContent = newNote;
        }

        // Returns 1 if x happened after y, -1 if y happened after x, 0 if same date
        public static int compareNotes(Note x, Note y)
        {
            return HarptosCalendar.FarthestInTime(x.date, y.date);
        }

        public int compareTo(Note n)
        {
            return compareNotes(this, n);
        }
    }
}