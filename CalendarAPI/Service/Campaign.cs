using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarAPI
{

    public class Campaign
    {
        string tag;
        string name;
        public List<Note> notes;
        public List<Timer> timers;
        string currentDate;

        bool ended;
        public bool IsEnded
        {
            get
            {
                return ended;
            }
            set
            {
                ended = value;
                SetEndNoteContent();
            }
        }
        public string Tag
        {
            get { return tag; }
            set { tag = fixTag(value); }
        }

        public string Name
        {
            get { return name; }
            set { ChangeName(value); }
        }

        public string CurrentDate
        {
            get { return currentDate; }
            set { setCurrentDate(value); }
        }

        public uint StartingNoteID { get; }
        public uint CurrentNoteID { get; }

        CalendarIDs IDManager { get; }

        public Campaign(string n, string t, string startDate, CalendarIDs IDManager) : this(n, t, startDate, startDate, IDManager)
        {
        }

        public Campaign(string n, string t, string startDate, string currDate, CalendarIDs IDManager)
        {
            notes = new List<Note>();
            timers = new List<Timer>();
            name = n;
            tag = t;
            this.IDManager = IDManager;
            if (Note.VerifyDate(startDate))
            {
                string msg = name + " began!";
                StartingNoteID = addNote(startDate, AlertScope.AllCampaigns, msg);
                currentDate = currDate;
            }
            if (Note.VerifyDate(currDate))
            {
                string msg = "Current Date";
                CurrentNoteID = addNote(currentDate, AlertScope.AllCampaigns, msg);
                currentDate = currDate;
            }
        }

        public Campaign(CalendarIDs IDManager)
        {
            notes = new List<Note>();
            timers = new List<Timer>();
            this.IDManager = IDManager;
        }
        public Campaign(dynamic campaignJson, CalendarIDs IDManager)
        {
            notes = new List<Note>();
            foreach (var note in campaignJson["notes"])
            {
                Note loadedNote = new Note(note);
                addNote(loadedNote);
            }

            timers = new List<Timer>();
            foreach (var timer in campaignJson["timers"])
            {
                Timer loadedTimer = new Timer(timer);
                addTimer(loadedTimer);
            }

            Tag = campaignJson["Tag"];
            Name = campaignJson["Name"];
            CurrentDate = campaignJson["CurrentDate"];
            this.IDManager = IDManager;
        }

        public void setCurrentDate(string newDate)
        {
            Note currOrEndNote = getCurrentDateOrEndNote();
            currOrEndNote.Date = newDate;
            currentDate = newDate;
            sortNotes();
        }

        public void ChangeName(string newName)
        {
            if (name == null)
                name = newName;
            else
                SetEndNoteContent();
        }

        private void SetEndNoteContent()
        {
            Note endNote = getCurrentDateOrEndNote();
            if (ended)
                endNote.Content = name + " ended.";
            else
                endNote.Content = "Current Date";
        }

        public void setStartDate(string newStartDate)
        {
            var startNote = getStartNote();
            startNote.Date = newStartDate;
        }

        public void setCurrentDate(int m, int d, int y)
        {
            setCurrentDate(String.Format("{0},{1},{2}", m.ToString(), d.ToString(), y.ToString()));
        }

        public uint addNote(string date, AlertScope importance, string content)
        {
            var note = new Note(IDManager.nextID(), date, importance, content, this);
            notes.Add(note);
            sortNotes();
            return note.ID;
        }

        public void addNote(Note noteToAdd)
        {
            addNote(noteToAdd.Date, noteToAdd.Importance, noteToAdd.Content);
        }

        public bool deleteNote(Note noteToDelete)
        {
            return notes.Remove(notes.Find(x => x.Equals(noteToDelete)));
        }

        public static string fixTag(string t)
        {
            if (t.Length > 10)
                return t.Substring(0, 10).ToUpper();
            else
                return t.ToUpper();
        }

        public Note getCurrentDateOrEndNote()
        {
            return findNote(CurrentNoteID);
        }

        public Note getStartNote()
        {
            return findNote(StartingNoteID);
        }

        public void SetDisplayValues()
        {
        }

        public void sortNotes()
        {
            notes.Sort(delegate (Note x, Note y)
            {
                return Note.compareDate(x, y);
            });
        }

        public Note findNote(uint id)
        {
            return notes.Find(n => n.ID == id);
        }

        public void addTimer(Timer t)
        {
            timers.Add(t);
        }

        public int hiddenTimerCount()
        {
            int count = 0;
            foreach (Timer t in timers)
                if (t.keepTrack == false)
                    count++;
            return count; ;
        }
    }
}
