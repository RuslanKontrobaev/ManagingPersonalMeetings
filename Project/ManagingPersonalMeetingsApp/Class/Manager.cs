using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManagingPersonalMeetingsApp.Class
{
    public static class Manager
    {
        private static Dictionary<string, Meeting> dict = new Dictionary<string, Meeting>()
        {
            ["t1"] = new Meeting("t1", DateTime.Now.AddMonths(-2).AddDays(-10), 10, DateTime.MinValue),
            ["t2"] = new Meeting("t2", DateTime.Now.AddDays(-10).AddHours(-5), 35, DateTime.Now.AddDays(-10).AddHours(-5).AddMinutes(-15)),
            ["t3"] = new Meeting("t3", DateTime.Now.AddSeconds(30), 20, DateTime.MinValue),
            ["t4"] = new Meeting("t4", DateTime.Now.AddHours(5).AddMinutes(15), 20, DateTime.Now.AddHours(5).AddMinutes(5)),
            ["t5"] = new Meeting("t5", DateTime.Now.AddHours(5).AddMinutes(45), 5, DateTime.MinValue),
        };
        private static readonly Timer timer = new Timer(new TimerCallback(CheckRemind), null, 0, 1000);

        public static void AddMeeting(Meeting meeting)
        {
            if (dict.ContainsKey(meeting.name))
            {
                Console.WriteLine("Встреча с таким названием уже запланирована. Поменяйте название");
                return;
            }
            if (WasIntersection(meeting.startTime, meeting.durationInMinutes))
            {
                Console.WriteLine("Введенное время встречи пересекается с уже существующими встречами. Измените время встречи");
                return;
            }
            if (meeting.remindTime >= meeting.startTime)
            {
                Console.WriteLine("Время напоминания можно задать только до начала встречи");
                return;
            }

            dict.Add(meeting.name, meeting);
        }

        public static bool WasIntersection(DateTime newStartTime, int newDuration)
        {
            DateTime newEndTime = newStartTime.AddMinutes(newDuration);
            var futureMeetings = dict.Values.Where(m => m.startTime > DateTime.Now).ToList();
            foreach (var m in futureMeetings)
            {
                DateTime curEndTimeOfMeeting = m.startTime.AddMinutes(m.durationInMinutes);
                if (newStartTime >= curEndTimeOfMeeting || newEndTime <= m.startTime)
                {
                    continue;
                }
                return true;
            }
            return false;
        }

        public static void ChangeMeeting(string prevName, string newName, DateTime newDate, int newDuration)
        {
            if (!dict.ContainsKey(prevName))
            {
                Console.WriteLine("Встреча с таким названием не найдена");
                return;
            }

            var saveDictItem = dict[prevName];
            dict.Remove(prevName);

            if (WasIntersection(newDate, newDuration))
            {
                Console.WriteLine("Введенное время встречи пересекается с уже существующими встречами. Измените время встречи");
                dict.Add(prevName, saveDictItem);
                return;
            }

            dict.Add(newName, new Meeting(newName, newDate, newDuration));
        }

        public static void ChangeMeeting(string prevName, string newName, DateTime newDate, int newDuration, DateTime newRemindTime)
        {
            if (!dict.ContainsKey(prevName))
            {
                Console.WriteLine("Встреча с таким названием не найдена");
                return;
            }

            var saveDictItem = dict[prevName];
            dict.Remove(prevName);

            if (WasIntersection(newDate, newDuration))
            {
                Console.WriteLine("Введенное время встречи пересекается с уже существующими встречами. Измените время встречи");
                dict.Add(prevName, saveDictItem);
                return;
            }

            dict.Add(newName, new Meeting(newName, newDate, newDuration, newRemindTime));
        }

        public static void DelMeeting(string key)
        {
            if (dict.ContainsKey(key))
            {
                dict.Remove(key);
                return;
            }
            Console.WriteLine("Встреча с таким названием не найдена");
        }

        public static void ShowFutureMeetings()
        {
            var num = GetNumberOfFutureMeetings();
            Console.WriteLine("\n***************************");
            Console.WriteLine($"Запланировано {num} встреч");

            if (num > 0)
                foreach (var meeting in GetFutureMeetings().OrderBy(m => m.startTime))
                {
                    Console.Write($"{meeting.name} {meeting.durationInMinutes} минут {meeting.startTime:d} в {meeting.startTime:T}");
                    if (meeting.remindTime != DateTime.MinValue)
                    {
                        Console.Write($"\tНапоминание: {meeting.remindTime:d} в {meeting.remindTime:t}");
                    }
                    Console.WriteLine();
                }     
        }

        public static void ShowMeetings()
        {
            var num = GetNumberOfMeetings();
            Console.WriteLine("\n***************************");
            Console.WriteLine($"Всего {num} встреч");

            if (num > 0)
                foreach (var meeting in GetMeetings().OrderBy(m => m.startTime))
                {
                    Console.Write($"{meeting.name} {meeting.durationInMinutes} минут {meeting.startTime:d} в {meeting.startTime:T}");
                    if (meeting.remindTime != DateTime.MinValue)
                    {
                        Console.Write($"\tНапоминание: {meeting.remindTime:d} в {meeting.remindTime:t}");
                    }
                    Console.WriteLine();
                }         
        }

        public static void ShowMeetings(DateTime date)
        {
            var num = GetNumberOfMeetings(date);
            Console.WriteLine("\n***************************");
            Console.WriteLine($"{date:d} всего {num} встреч");

            if (num > 0)
                foreach (var meeting in GetMeetings(date).OrderBy(m => m.startTime))
                {
                    Console.Write($"{meeting.name} {meeting.durationInMinutes} минут {meeting.startTime:d} в {meeting.startTime:T}");
                    if (meeting.remindTime != DateTime.MinValue)
                    {
                        Console.Write($"\tНапоминание: {meeting.remindTime:d} в {meeting.remindTime:t}");
                    }
                    Console.WriteLine();
                }   
        }

        public static int GetNumberOfFutureMeetings() => GetFutureMeetings().Count();

        public static int GetNumberOfMeetings() => GetMeetings().Count();

        public static int GetNumberOfMeetings(DateTime date) => GetMeetings(date).Count();

        public static List<Meeting> GetFutureMeetings() => dict.Values.Where(m => m.startTime >= DateTime.Now).ToList();

        public static List<Meeting> GetMeetings() => dict.Values.ToList();

        public static List<Meeting> GetMeetings(DateTime date) => dict.Values.Where(m => m.startTime.Date == date.Date).ToList();

        public static void CheckRemind(Object obj)
        {
            foreach (var meeting in GetFutureMeetings())
            {
                if (meeting.remindTime == DateTime.MinValue)
                    continue;

                if ((meeting.remindTime.Year == DateTime.Now.Year) &&
                    (meeting.remindTime.Month == DateTime.Now.Month) &&
                    (meeting.remindTime.Day == DateTime.Now.Day) &&
                    (meeting.remindTime.Hour == DateTime.Now.Hour) &&
                    (meeting.remindTime.Minute == DateTime.Now.Minute) &&
                    (meeting.remindTime.Second == DateTime.Now.Second))
                {
                    Console.WriteLine("\n*************Напоминание*************");
                    Console.WriteLine("");
                    Console.WriteLine($"{meeting.name} {meeting.durationInMinutes} минут {meeting.startTime:d} в {meeting.startTime:T}");
                    Console.WriteLine("*************************************");

                }
            }
        }

        public static void ExportToFile(List<Meeting> meetings, DateTime date)
        {
            using (StreamWriter writer = new StreamWriter($"meetings {date:d}.txt", false))
            {
                string text = $"Дата: {date:d}\nКоличество встреч: {meetings.Count()}\n";
                writer.Write(text);

                if (meetings.Count() == 0) return;

                foreach (var meeting in meetings)
                {
                    writer.WriteLine($"{meeting.name} {meeting.durationInMinutes} минут {meeting.startTime:d} в {meeting.startTime:T}");
                    if (meeting.remindTime != DateTime.MinValue)
                    {
                        writer.WriteLine($"\tНапоминание: {meeting.remindTime:d} в {meeting.remindTime:t}");
                    }
                }   
            }
        }
    }
}