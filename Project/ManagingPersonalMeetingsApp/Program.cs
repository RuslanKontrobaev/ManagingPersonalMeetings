using ManagingPersonalMeetingsApp.Class;
using System;

namespace ManagingPersonalMeetingsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Вас привествует программа для управления личными встречами\n" +
                              "Для добавления встречи введите команду, название, дату и продолжительность встречи в минутах, дату напоминания (опционально) в формате \"new Название ДД.ММ.ГГГГ ЧЧ:ММ:СС 0 [ДД.ММ.ГГГГ ЧЧ:ММ:СС]\"\n" +
                              "Чтобы увидеть все встречи, введите команду \"showall\"\n" +
                              "Чтобы увидеть будущие встречи, введите команду \"show\", либо \"show ДД.ММ.ГГГГ\", чтобы увидеть встречи за конкретный день" +
                              "Чтобы изменить встречу, введите команду, старое название, новое название, новую дату, новую продолжительность встречи, время напоминания (опционально) в формате \"edit Старое название Новое название ДД.ММ.ГГГГ ЧЧ:ММ:СС 0 [ДД.ММ.ГГГГ ЧЧ:ММ:СС]\"\n" +
                              "Чтобы удалить встречу, введите команду в формате \"delete Название\"\n" +
                              "Чтобы экспортировать расписание встреч за конкретный день в текстовый файл, введите команду \"save ДД.ММ.ГГГГ\"" +
                              "Чтобы завершить работу программы, введите команду \"quit\"\n");

            var input = Console.ReadLine();
            while (!(input.ToLower() == "quit") && !(input == ""))
            {
                if (input.ToLower().StartsWith("showall"))
                {
                    Manager.ShowMeetings();
                }
                else if (input.ToLower().StartsWith("show"))
                {
                    var words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 1)
                    {
                        Manager.ShowFutureMeetings();
                    }
                    else if (words.Length == 2)
                    {
                        DateTime date;
                        bool isSuccess = DateTime.TryParse(words[1], out date);
                        if (!isSuccess)
                        {
                            Console.WriteLine("Некорректный ввод");
                            input = Console.ReadLine();
                            continue;
                        }
                        Manager.ShowMeetings(date);
                    }
                    else
                        Console.WriteLine("Введены некорректные данные");
                }
                else if (input.ToLower().StartsWith("edit"))
                {
                    var words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 8) //Со временем напоминания
                    {
                        string prevName = words[1];
                        string newName = words[2];

                        DateTime newDate;
                        bool isSuccess = DateTime.TryParse(words[3] + " " + words[4], out newDate);
                        if (!isSuccess)
                        {
                            Console.WriteLine("Некорректный ввод");
                            input = Console.ReadLine();
                            continue;
                        }

                        int newDuration = Convert.ToInt32(words[5]);

                        DateTime newRemindTime;
                        isSuccess = DateTime.TryParse(words[6] + " " + words[7], out newRemindTime);
                        if (!isSuccess)
                        {
                            Console.WriteLine("Некорректный ввод");
                            input = Console.ReadLine();
                            continue;
                        }

                        Manager.ChangeMeeting(prevName, newName, newDate, newDuration, newRemindTime);
                    }
                    else if (words.Length == 6) //Без временем напоминания
                    {
                        string prevName = words[1];
                        string newName = words[2];

                        DateTime newDate;
                        bool isSuccess = DateTime.TryParse(words[3] + " " + words[4], out newDate);
                        if (!isSuccess)
                        {
                            Console.WriteLine("Некорректный ввод");
                            input = Console.ReadLine();
                            continue;
                        }

                        int newDuration = Convert.ToInt32(words[5]);
                        Manager.ChangeMeeting(prevName, newName, newDate, newDuration);
                    }
                    else
                        Console.WriteLine("Некорректный ввод");
                }
                else if (input.ToLower().StartsWith("delete"))
                {
                    var words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 2)
                    {
                        Manager.DelMeeting(words[1]);
                    }
                    else
                        Console.WriteLine("Некорректный ввод");
                }
                else if (input.ToLower().StartsWith("save"))
                {
                    var words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 2)
                    {
                        DateTime date = DateTime.Parse(words[1]);
                        Manager.ExportToFile(Manager.GetMeetings(date), date);
                    }
                    else
                        Console.WriteLine("Некорректный ввод");
                }
                else if (input.ToLower().StartsWith("new"))
                {
                    var words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 5) //Без времени напоминания
                    {
                        DateTime newStartTime;
                        bool isSuccess = DateTime.TryParse(words[2] + " " + words[3], out newStartTime);
                        if (!isSuccess)
                        {
                            Console.WriteLine("Некорректный ввод");
                            input = Console.ReadLine();
                            continue;
                        }
                        if (newStartTime < DateTime.Now)
                        {
                            Console.WriteLine("Совершена попытка запланировать встречу на прошлое время, задайте будущее время");
                            input = Console.ReadLine();
                            continue;
                        }
                        int newDuration = Convert.ToInt32(words[4]);

                        Manager.AddMeeting(new Meeting(words[1], newStartTime, newDuration));
                    }
                    else if (words.Length == 7) //Со временем напоминания
                    {
                        DateTime newStartTime;
                        bool isSuccess = DateTime.TryParse(words[2] + " " + words[3], out newStartTime);
                        if (!isSuccess)
                        {
                            Console.WriteLine("Некорректный ввод");
                            input = Console.ReadLine();
                            continue;
                        }
                        if (newStartTime < DateTime.Now)
                        {
                            Console.WriteLine("Совершена попытка запланировать встречу на прошлое время, задайте будущее время");
                            input = Console.ReadLine();
                            continue;
                        }
                        int newDuration = Convert.ToInt32(words[4]);

                        DateTime newRemindTime;
                        isSuccess = DateTime.TryParse(words[5] + " " + words[6], out newRemindTime);
                        if (!isSuccess)
                        {
                            Console.WriteLine("Некорректный ввод");
                            input = Console.ReadLine();
                            continue;
                        }

                        Manager.AddMeeting(new Meeting(words[1], newStartTime, newDuration, newRemindTime));
                    }
                    else
                        Console.WriteLine("Некорректный ввод");                    
                }
                else
                    Console.WriteLine("Введены некорректные данные");

                input = Console.ReadLine();
            }
        }
    }
}