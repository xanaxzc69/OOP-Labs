using System;
using System.Globalization;
using System.Text;

namespace ClinicApp;

class Program
{
    static PatientManager patientManager = new PatientManager();
    static DoctorManager doctorManager = new DoctorManager();
    static bool inputClosed = false;

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

        patientManager.Add(p1);
        patientManager.Add(p2);
        patientManager.Add(p3);
        patientManager.Add(p4);
        patientManager.Add(p5);
        patientManager.DisplayAll();
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

        doctorManager.Add(d1);
        doctorManager.Add(d2);
        doctorManager.Add(d3);
        doctorManager.Add(d4);
        doctorManager.DisplayAll();
        Console.WriteLine();
        MainMenu();
        Console.WriteLine("До побачення!");
    }

    // ===== Головне меню =====
    static void MainMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("===== ГОЛОВНЕ МЕНЮ =====");
            Console.WriteLine("1. Пацієнти");
            Console.WriteLine("2. Лікарі");
            Console.WriteLine("0. Вихід");
            string choice = ReadText("Ваш вибір: ");
            if (inputClosed || choice == "0") return;

            if (choice == "1")
            {
                PatientsMenu();
            }
            else if (choice == "2")
            {
                DoctorsMenu();
            }
            else
            {
                Console.WriteLine("Невідомий пункт меню.");
            }
        }
    }

    // ===== Підменю «Пацієнти» =====
    static void PatientsMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- Пацієнти ---");
            Console.WriteLine("1. Показати всіх");
            Console.WriteLine("2. Додати");
            Console.WriteLine("3. Знайти за ім'ям");
            Console.WriteLine("4. Видалити");
            Console.WriteLine("5. Статистика");
            Console.WriteLine("0. Назад");
            string choice = ReadText("Ваш вибір: ");
            if (inputClosed || choice == "0") return;

            if (choice == "1")
            {
                patientManager.DisplayAll();
            }
            else if (choice == "2")
            {
                string firstName = ReadText("Ім'я: ");
                string lastName = ReadText("Прізвище: ");
                DateTime? dob = ReadDate("Дата народження (дд.мм.рррр): ");
                if (dob == null) continue;
                string bloodType = ReadText("Група крові (напр. A+): ");
                string phone = ReadText("Телефон: ");
                if (firstName.Length == 0 || lastName.Length == 0)
                {
                    Console.WriteLine("Ім'я та прізвище не можуть бути порожніми.");
                    continue;
                }
                patientManager.Add(new Patient(firstName, lastName, dob.Value, bloodType, phone));
            }
            else if (choice == "3")
            {
                string query = ReadText("Введіть ім'я або прізвище: ");
                Patient[] found = patientManager.FindByName(query);
                if (found.Length == 0)
                {
                    Console.WriteLine("Нікого не знайдено.");
                }
                for (int i = 0; i < found.Length; i++)
                {
                    Console.WriteLine(found[i]);
                }
            }
            else if (choice == "4")
            {
                int? id = ReadInt("ID пацієнта: ");
                if (id == null) continue;
                if (patientManager.Remove(id.Value))
                    Console.WriteLine("Пацієнта видалено.");
                else
                    Console.WriteLine("Пацієнта з таким ID не знайдено.");
            }
            else if (choice == "5")
            {
                patientManager.DisplayStats();
            }
            else
            {
                Console.WriteLine("Невідомий пункт меню.");
            }
        }
    }

    // ===== Підменю «Лікарі» =====
    static void DoctorsMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- Лікарі ---");
            Console.WriteLine("1. Показати всіх");
            Console.WriteLine("2. Додати");
            Console.WriteLine("3. Знайти за спеціальністю");
            Console.WriteLine("4. Видалити");
            Console.WriteLine("5. Статистика");
            Console.WriteLine("0. Назад");
            string choice = ReadText("Ваш вибір: ");
            if (inputClosed || choice == "0") return;

            if (choice == "1")
            {
                doctorManager.DisplayAll();
            }
            else if (choice == "2")
            {
                string firstName = ReadText("Ім'я: ");
                string lastName = ReadText("Прізвище: ");
                string speciality = ReadText("Спеціальність: ");
                string license = ReadText("Номер ліцензії: ");
                string phone = ReadText("Телефон: ");
                int? start = ReadInt("Година початку роботи (0-23): ");
                if (start == null) continue;
                int? end = ReadInt("Година кінця роботи (0-23): ");
                if (end == null) continue;
                if (start.Value < 0 || start.Value > 23 || end.Value < 0 || end.Value > 23 || start.Value >= end.Value)
                {
                    Console.WriteLine("Некоректний графік: години 0-23, початок раніше кінця.");
                    continue;
                }
                Doctor doctor = new Doctor(firstName, lastName, speciality, license, phone);
                doctor.WorkStartHour = start.Value;
                doctor.WorkEndHour = end.Value;
                doctorManager.Add(doctor);
            }
            else if (choice == "3")
            {
                string query = ReadText("Спеціальність: ");
                Doctor[] found = doctorManager.FindBySpeciality(query);
                if (found.Length == 0)
                {
                    Console.WriteLine("Лікарів такої спеціальності не знайдено.");
                }
                for (int i = 0; i < found.Length; i++)
                {
                    Console.WriteLine(found[i]);
                }
            }
            else if (choice == "4")
            {
                int? id = ReadInt("ID лікаря: ");
                if (id == null) continue;
                if (doctorManager.Remove(id.Value))
                    Console.WriteLine("Лікаря видалено.");
                else
                    Console.WriteLine("Лікаря з таким ID не знайдено.");
            }
            else if (choice == "5")
            {
                doctorManager.DisplayStats();
            }
            else
            {
                Console.WriteLine("Невідомий пункт меню.");
            }
        }
    }

    // ===== Допоміжні методи вводу =====
    static string ReadText(string prompt)
    {
        Console.Write(prompt);
        string line = Console.ReadLine();
        if (line == null)
        {
            inputClosed = true;   // ввід закінчився — меню завершиться
            return "";
        }
        return line.Trim();
    }

    static int? ReadInt(string prompt)
    {
        string text = ReadText(prompt);
        int value;
        if (int.TryParse(text, out value)) return value;
        if (!inputClosed) Console.WriteLine("Потрібно ввести ціле число.");
        return null;
    }

    static DateTime? ReadDate(string prompt, bool emptyMeansTomorrow = false)
    {
        string text = ReadText(prompt);
        if (text.Length == 0 && emptyMeansTomorrow && !inputClosed) return DateTime.Today.AddDays(1);
        DateTime value;
        if (DateTime.TryParseExact(text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            return value;
        if (!inputClosed) Console.WriteLine("Некоректна дата. Формат: дд.мм.рррр");
        return null;
    }
}
