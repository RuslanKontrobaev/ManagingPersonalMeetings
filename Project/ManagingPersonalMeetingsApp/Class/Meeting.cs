using System;

namespace ManagingPersonalMeetingsApp.Class
{
    public class Meeting
    {
        public string name { get; set; }
        public DateTime startTime { get; set; }
        public int durationInMinutes { get; set; }
        public DateTime remindTime { get; set; }

        public Meeting(string name, DateTime startTime, int durationInMinutes)
        {
            this.name = name;
            this.startTime = startTime;
            this.durationInMinutes = durationInMinutes;
            this.remindTime = DateTime.MinValue;
        }

        public Meeting(string name, DateTime startTime, int durationInMinutes, DateTime remindTime)
        {
            this.name = name;
            this.startTime = startTime;
            this.durationInMinutes = durationInMinutes;
            this.remindTime = remindTime;
        }
    }
}