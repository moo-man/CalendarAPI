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

        public Campaign(string n, string t, string startDate) : this(n, t, startDate, startDate)
        {

        }

        public Campaign(string n, string t, string startDate, string currDate)
        {
            notes = new List<Note>();
            timers = new List<Timer>();
            name = n;
            tag = t;
            if (Note.VerifyDate(startDate))
            {
                string msg = name + " began!";
                addNote(startDate, AlertScope.global, msg);
                currentDate = currDate;
            }
            if (Note.VerifyDate(currDate))
            {
                string msg = "Current Date";
                addNote(currentDate, AlertScope.global, msg);
                currentDate = currDate;
            }
        }

        public Campaign()
        {
            notes = new List<Note>();
            timers = new List<Timer>();
        }
        public Campaign(dynamic campaignJson)
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
            {
                Note endNote = getCurrentDateOrEndNote();
                if (isEnded())
                    endNote.Content= newName + " ended.";
                else
                    endNote.Content = "Current Date";

                Note startNote = notes.Find(x => x.Content == name + " began!");
                startNote.Content = newName + " began!";

                name = newName;
            }
        }

        public void setStartDate(string newStartDate)
        {
            Note startNote = notes.Find(x => x.Content == name + " began!");
            startNote.Content = newStartDate;
        }

        /*public void setStartDate(string newStartDate, string newCampaigName)
        {
            Note startNote = notes.Find(x => x.Content == name + " began!");
            startNote.editDate(newStartDate);
            startNote.editContent(newCampaigName + " began!");
        }*/

        public void setCurrentDate(int m, int d, int y)
        {
            StringBuilder newDate = new StringBuilder();
            if (m < 10)
                newDate.Append("0" + m);
            else
                newDate.Append(m);

            if (d < 10)
                newDate.Append("0" + d);
            else
                newDate.Append(d);

            string yString = y.ToString();
            while (yString.Length < 4)
                yString.Insert(0, "0");

            newDate.Append(yString);

            setCurrentDate(String.Format("{0},{1},{2}", m.ToString("00"), d.ToString("00"), y.ToString()));
        }

        public Note returnBeginNote()
        {
            return notes.Find(x => x.Content == Name + " began!");
        }

        #region starting/ending campaign
        public void endCampaign()
        {
            Note endNote = findNote("Current Date");
            if (endNote == null)
                return;
            endNote.Content = endNote.Campaign.Name + " ended.";
        }

        public void startCampaign()
        {
            Note endNote = findNote(Name + " ended.");
            if (endNote == null)
                return;
            endNote.Content = "Current Date";
        }

        public bool isEnded()
        {
            if (findNote("Current Date") == null && findNote(Name + " ended.") != null)
                return true;

            else return false;
        }

        public void toggleEnded()
        {
            if (isEnded())
                startCampaign();
            else
                endCampaign();
        }
        #endregion

        public void addNote(string date, AlertScope importance, string note)
        {
            notes.Add(new Note(date, importance, note, this));
            sortNotes();
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

        // Returns the note "Current Date" or "ended"
        public Note getCurrentDateOrEndNote()
        {
            if (isEnded())
                return findNote(Name + " ended.");
            else
                return findNote("Current Date");
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

        public Note findNote(string content)
        {
            foreach (Note n in notes)
                if (n.Content == content)
                    return n;
            return null;
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
