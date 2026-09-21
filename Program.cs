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
    }
}
