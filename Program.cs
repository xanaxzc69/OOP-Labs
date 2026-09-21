using System;
using System.Globalization;
using System.Text;

namespace ClinicApp;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        // У консолі Windows кирилиця коректно читається лише в Unicode
        if (OperatingSystem.IsWindows() && !Console.IsInputRedirected)
            Console.InputEncoding = Encoding.Unicode;
        else
            Console.InputEncoding = Encoding.UTF8;

        // 5 пацієнтів, створених різними конструкторами
        Patient p1 = new Patient();                                                                  // без параметрів
        Patient p2 = new Patient("Іван", "Петренко");                                                // (firstName, lastName)
        Patient p3 = new Patient("Олена", "Коваль", new DateTime(1985, 3, 14), "A+", "0671234567");   // повний
        Patient p4 = new Patient("Марія", "Шевченко", new DateTime(2015, 11, 2), "O+", "0509876543"); // повний
        Patient p5 = new Patient("Петро", "Бондар", new DateTime(1955, 7, 20), "B-", "0931112233");   // повний
        p3.Email = "olena.koval@gmail.com";

        Console.WriteLine("=== Пацієнти ===");
        Console.WriteLine(p1);
        Console.WriteLine(p2);
        Console.WriteLine(p3);
        Console.WriteLine(p4);
        Console.WriteLine(p5);
        Console.WriteLine($"{p3.FullName}: дорослий? {p3.IsAdult}, email: {p3.Email}");
        Console.WriteLine($"{p4.FullName}: дорослий? {p4.IsAdult}");
        Console.WriteLine();

        // Лікарі різними конструкторами
        Doctor d1 = new Doctor("Сергій", "Литвин", "Терапевт", "LIC-1001", "0671112233");
        Doctor d2 = new Doctor("Ірина", "Ткач", "Кардіолог", "LIC-1002", "0502223344");
        Doctor d3 = new Doctor("Андрій", "Гнатюк", "Хірург");
        Doctor d4 = new Doctor();

        // Графік можна змінити після створення (публічні сетери)
        d2.WorkStartHour = 10;
        d2.WorkEndHour = 18;
        d3.WorkStartHour = 14;
        d3.WorkEndHour = 22;
        d4.WorkStartHour = 0;
        d4.WorkEndHour = 23;

        Doctor[] doctors = { d1, d2, d3, d4 };
        Console.WriteLine("=== Лікарі ===");
        for (int i = 0; i < doctors.Length; i++)
        {
            Console.WriteLine(doctors[i]);
        }

        Console.WriteLine();
        Console.WriteLine($"Зараз {DateTime.Now.ToString("HH:mm")}. Доступні лікарі:");
        int availableCount = 0;
        for (int i = 0; i < doctors.Length; i++)
        {
            if (doctors[i].IsAvailableNow)
            {
                Console.WriteLine($"  + {doctors[i].FullName} ({doctors[i].Speciality})");
                availableCount++;
            }
        }
        if (availableCount == 0)
        {
            Console.WriteLine("  зараз ніхто не працює");
        }
        Console.WriteLine($"Чи приймає {d1.FullName} о 12:00? {d1.CanAcceptAt(12)}");
        Console.WriteLine($"Чи приймає {d1.FullName} о 20:00? {d1.CanAcceptAt(20)}");
        Console.WriteLine();
    }
}
