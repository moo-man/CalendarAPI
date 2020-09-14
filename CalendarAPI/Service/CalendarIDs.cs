namespace CalendarAPI
{
    public class CalendarIDs
    {
        uint id;
        public CalendarIDs(uint ID = 0)
        {
            id = ID;
        }

        public uint nextID()
        {
            return id++;
        }
    }
}