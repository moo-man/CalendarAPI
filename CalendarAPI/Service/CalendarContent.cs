using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarAPI
{
    // Notes have 3 levels of importance
    // dontAlert will not alert any campaign when that date is reached
    // alertCampaign will alert the campaign that the note belongs to
    // alertAll will alert all campaigns in this calendar
    public enum AlertScope { OnlyOnDateInCampaign, OnlyInCampaign, AllCampaigns}

    public class CalendarContents
    {
        public HarptosCalendar calendar;

        public List<Campaign> CampaignList { get; set; }
        
        public List<Note> GeneralNoteList { get; set; }

        public Campaign ActiveCampaign { get; set; }

        CalendarIDs IDManager { get;}

        public CalendarContents()
        {
            ActiveCampaign = null;
            calendar = new HarptosCalendar();
            CampaignList = new List<Campaign>();
            GeneralNoteList = new List<Note>();
            IDManager = new CalendarIDs();
        }

        public CalendarContents(dynamic json) : this()
        {

            foreach (var campaign in json["CampaignList"])
                AddCampaign(new Campaign(campaign));

            foreach (var note in json["GeneralNoteList"])
            {
                Note loadedNote = new Note(note);
                loadedNote.Campaign = null;
                AddGeneralNote(loadedNote);
            }
        }

        public void addNewCampaign(string name, string t, string startDate)
        {
            Campaign newCampaign = new Campaign(name, t, startDate, IDManager);
            CampaignList.Add(newCampaign);
        }

        public void addLoadedCampaign(string name, string t, string currentDate)
        {
            // since the constructor of campaign requires start date (then adds a note of start date)
            // we have to start a blank campaign and add to that, kinda gross but whatev
            Campaign loadedCampaign = new Campaign(IDManager);
            loadedCampaign.Name = name;
            loadedCampaign.Tag = t;
            loadedCampaign.CurrentDate = currentDate;
            CampaignList.Add(loadedCampaign);
        }

        public void AddGeneralNote(Note noteToAdd)
        {
            if (noteToAdd.Campaign != null)
                return;
            else
                GeneralNoteList.Add(noteToAdd);
        }

        public void AddNote(Note noteToAdd)
        {
            if (noteToAdd.Campaign == null)
            {
                GeneralNoteList.Add(noteToAdd);
                GeneralNoteList.Sort(delegate (Note x, Note y)
                {
                    return Note.compareDate(x, y);
                });
            }
            else
                noteToAdd.Campaign.addNote(noteToAdd);
        }

        public void AddCampaign(Campaign c)
        {
            CampaignList.Add(c);
        }

        public bool deleteCampaign(string tag)
        {
            return CampaignList.Remove(CampaignList.Find(x => x.Tag == tag));
        }

        public int numOfCampaigns()
        {
            return CampaignList.Count();
        }

        public Campaign setActiveCampaign(int index)
        {
            return setActiveCampaign(CampaignList[index]);
        }

        public Campaign setActiveCampaign(string tag)
        {
            return setActiveCampaign(CampaignList.FindIndex(c => c.Tag == tag));
        }

        public Campaign setActiveCampaign(Campaign c)
        {
            if (CampaignList.Contains(c))
            {
                ActiveCampaign = c;
                var splitDate = ActiveCampaign.CurrentDate.Split(",");
                calendar = new HarptosCalendar(Int32.Parse(splitDate[0]),Int32.Parse(splitDate[1]),Int32.Parse(splitDate[2]));
            }
            else
                ActiveCampaign = null;

            return ActiveCampaign;
        }

        /// <summary>
        /// goToCurrentDate() function sets the HarptosCalendar to the current date of the ACTIVE campaign
        /// </summary>
        public void goToCurrentDate()
        {
            if (ActiveCampaign == null)
                return;
            calendar.setDate(ActiveCampaign.CurrentDate);
        }

        #region Forward in time
        /// <summary>
        /// Move to the next day in the calendar
        /// </summary>
        public List<Tuple<Note, string>> addDay()
        {
            calendar.addDay();

            List<Tuple<Note, string>> notesAndDate = new List<Tuple<Note, string>>();

            List<Note> notesOnThisDay = findNotesToList();
            foreach (Note n in notesOnThisDay)
            {
                notesAndDate.Add(new Tuple<Note, string>(n, this.calendar.ToString()));
            }

            return notesAndDate;
        }

        /// <summary>
        /// Move to the next n days in the calendar
        /// </summary>
        /// <param name="num">The number of days passing</param>
        public List<Tuple<Note, string>> addDay(int num)
        {
            List<Tuple<Note, string>> notesAndDate = new List<Tuple<Note, string>>();
            for (int i = 0; i < num; i++)
            {
                notesAndDate.AddRange(addDay());
            }
            return notesAndDate;
        }

        public List<Tuple<Note, string>> addTenday()
        {
            return addDay(10);
        }

        public List<Tuple<Note, string>> addMonth()
        {
            return addDay(30);
        }

        public List<Tuple<Note, string>> addMonth(int num)
        {
            List<Tuple<Note, string>> notesAndDate = new List<Tuple<Note, string>>();
            for (int i = 0; i < num; i++)
                notesAndDate.AddRange(addMonth());
            return notesAndDate;
        }

        public void addYear()
        {
            calendar.addYear();
        }

        public void addYear(int num)
        {
            for (int i = 0; i < num; i++)
                addYear();
        }
        #endregion

        #region Backward in time
        public void subDay()
        {
            calendar.subDay();

        }

        public void subDay(int num)
        {
            for (int i = 0; i < num; i++)
                subDay();
        }

        public void subTenday()
        {
            subDay(10);
        }

        public void subMonth()
        {
            subDay(30);
        }

        public void subYear()
        {
            calendar.subYear();
        }
        #endregion


        /// <summary>
        /// Finds all notes that should be listed based on the current date of the calendar
        /// </summary>
        /// <returns></returns>
        public List<Note> findNotesToList()
        {
            List<Note> listOfNotes = new List<Note>();

            foreach (Note n in GeneralNoteList)
            {
                if (n.Importance == AlertScope.AllCampaigns && calendar.isAnniversary(n.Date) || (n.Importance == AlertScope.OnlyOnDateInCampaign && calendar.sameDate(n.Date)))
                    listOfNotes.Add(n);
            }

            foreach (Campaign c in CampaignList)
            {
                foreach (Note n in c.notes)
                {
                    if (c.Equals(ActiveCampaign)) // If the note belongs to current campaign, and has appropriate visibilty, and is anniversary of this date
                    {
                        if ((n.Importance == AlertScope.OnlyInCampaign || n.Importance == AlertScope.AllCampaigns) && calendar.isAnniversary(n.Date))
                        {
                            if (n.Content.Equals("Current Date") == false) // don't print the current date of current campaign, as that is always the current date
                                listOfNotes.Add(n);
                        }
                        else if (n.Importance == AlertScope.OnlyOnDateInCampaign && calendar.sameDate(n.Date))
                            listOfNotes.Add(n);
                    }

                    else // If the note does not belong in the current campaign
                        if ((n.Importance == AlertScope.AllCampaigns) && calendar.isAnniversary(n.Date)) // if the note happened on this day and is of                                                                                        // sufficient importance level
                        listOfNotes.Add(n);
                } // end foreach note
            } // end foreach campaign

            return listOfNotes;
        }

        private string ReturnRelativity(Note n)
        {
            if (calendar.yearsAgo(n.Date) == 1)
                return (" (" + calendar.yearsAgo(n.Date) + " year ago)\n");
            else if (calendar.yearsAgo(n.Date) > 1)                                                    // Note happened in past
                return (" (" + calendar.yearsAgo(n.Date) + " years ago)\n");
            else if ((calendar.yearsAgo(n.Date) == 0))
                return ("\n");                                                                         // Note happened this very day
            else if (calendar.yearsAgo(n.Date) == -1)
                return (" (in " + Math.Abs(calendar.yearsAgo(n.Date)) + " year)\n");
            else if (calendar.yearsAgo(n.Date) < -1)                                                   // Note happens in future
                return (" (in " + Math.Abs(calendar.yearsAgo(n.Date)) + " years)\n");
            else
                return ("Error.");
        }

        public List<Timer> findTimersToList()
        {
            List<Timer> listOfTimers = new List<Timer>();

            //if (ActiveCampaign != null && ActiveCampaign.timers != null)
            if (ActiveCampaign != null)
            {

                foreach (Timer t in ActiveCampaign.timers)
                {
                    if (t.keepTrack && calendar.sameDate(t.Date) == false)
                    {
                        listOfTimers.Add(t);
                    }
                }
            }
            return listOfTimers;
        }

        public List<Note> returnMonthNotes()
        {
            List<Note> listOfNotes = new List<Note>();

            foreach (Note n in GeneralNoteList)
            {
                if (calendar.sameMonth(n.Date))
                    listOfNotes.Add(n);
            }

            foreach (Campaign c in CampaignList)
            {
                foreach (Note n in c.notes)
                {
                    if (c.Equals(ActiveCampaign))
                    {
                        if (calendar.sameMonth(n.Date))
                            listOfNotes.Add(n);
                    }
                    else
                    {
                        if (n.Importance == AlertScope.AllCampaigns && calendar.sameMonth(n.Date))
                            listOfNotes.Add(n);
                    }
                } 
            } 

            return listOfNotes;

        }

        public Note findNote(string content, noteType type)
        {
            if (type == noteType.generalNote)
            {
                foreach (Note n in GeneralNoteList)
                    if (n.Content == content)
                        return n;
            }

            else
            {
                foreach (Campaign c in CampaignList)
                    foreach (Note n in c.notes)
                        if (n.Content == content)
                            return n;
            }
            return null;
        }

        public void deleteNote(string content, noteType type)
        {
            Note noteToDelete = findNote(content, type);
            deleteNote(noteToDelete);
        }

        public void deleteNote(Note noteToDelete)
        {
            if (noteToDelete == null)
                return;

            if (noteToDelete.Campaign == null)
                GeneralNoteList.Remove(noteToDelete);
            else
                noteToDelete.Campaign.deleteNote(noteToDelete);
        }
        public static bool CanEditOrDelete(Note noteToTest)
        {
            if (noteToTest == null)
                return false;

            if (noteToTest.Campaign != null &&                                          // If campaign is not null (note not general)
        (noteToTest.Campaign.getCurrentDateOrEndNote() == noteToTest ||   // AND (the note is not the currentdate note OR the begin note)
        noteToTest.Campaign.getStartNote() == noteToTest))
                return false;
            else
                return true;
        }

        public void saveButton()
        {
            //Utility.Save(this);
        }
    }
}
