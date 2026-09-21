using System;
using System.Globalization;
using System.Text;

namespace ClinicApp;

class Program
{
    // Уся робота програми — через єдиний об'єкт clinic
    static Clinic clinic = new Clinic("Медична клініка «Здоров'я»");
    static bool inputClosed = false;

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        // У консолі Windows кирилиця коректно читається лише в Unicode
        if (OperatingSystem.IsWindows() && !Console.IsInputRedirected)
            Console.InputEncoding = Encoding.Unicode;
        else
            Console.InputEncoding = Encoding.UTF8;

        Console.WriteLine($"=== {clinic.Name} ===");
        // 5 пацієнтів, створених різними конструкторами
        Patient p1 = new Patient();                                                                  // без параметрів
        Patient p2 = new Patient("Іван", "Петренко");                                                // (firstName, lastName)
        Patient p3 = new Patient("Олена", "Коваль", new DateTime(1985, 3, 14), "A+", "0671234567");   // повний
        Patient p4 = new Patient("Марія", "Шевченко", new DateTime(2015, 11, 2), "O+", "0509876543"); // повний
        Patient p5 = new Patient("Петро", "Бондар", new DateTime(1955, 7, 20), "B-", "0931112233");   // повний
        p3.Email = "olena.koval@gmail.com";

        clinic.Patients.Add(p1);
        clinic.Patients.Add(p2);
        clinic.Patients.Add(p3);
        clinic.Patients.Add(p4);
        clinic.Patients.Add(p5);
        clinic.Patients.DisplayAll();
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

        clinic.Doctors.Add(d1);
        clinic.Doctors.Add(d2);
        clinic.Doctors.Add(d3);
        clinic.Doctors.Add(d4);
        clinic.Doctors.DisplayAll();
        Console.WriteLine();
        // Записи на завтра (майбутні)
        DateTime tomorrow = DateTime.Today.AddDays(1);
        clinic.Appointments.Book(1, 1, tomorrow.AddHours(9));
        clinic.Appointments.Book(3, 2, tomorrow.AddHours(11), 45);
        clinic.Appointments.Book(5, 3, tomorrow.AddHours(15));
        clinic.Appointments.Book(2, 1, tomorrow.AddHours(10), 20);
        clinic.Appointments.Book(4, 2, tomorrow.AddHours(12));

        // Некоректні ID — менеджер їх відхиляє
        clinic.Appointments.Book(99, 1, tomorrow.AddHours(13));
        clinic.Appointments.Book(1, 99, tomorrow.AddHours(13));

        clinic.Appointments.Cancel(4, "Пацієнт переніс візит");
        Console.WriteLine();
        clinic.DisplaySchedule(tomorrow);
        Console.WriteLine();
        clinic.GenerateReport();
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
            Console.WriteLine("3. Записи");
            Console.WriteLine("4. Розклад на дату");
            Console.WriteLine("5. Звіт клініки");
            Console.WriteLine("6. Зростаючий масив (демонстрація)");
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
            else if (choice == "3")
            {
                AppointmentsMenu();
            }
            else if (choice == "4")
            {
                ScheduleMenu();
            }
            else if (choice == "5")
            {
                clinic.GenerateReport();
            }
            else if (choice == "6")
            {
                DemoGrowable();
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
                clinic.Patients.DisplayAll();
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
                clinic.Patients.Add(new Patient(firstName, lastName, dob.Value, bloodType, phone));
            }
            else if (choice == "3")
            {
                string query = ReadText("Введіть ім'я або прізвище: ");
                Patient[] found = clinic.Patients.FindByName(query);
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
                if (clinic.Patients.Remove(id.Value))
                    Console.WriteLine("Пацієнта видалено.");
                else
                    Console.WriteLine("Пацієнта з таким ID не знайдено.");
            }
            else if (choice == "5")
            {
                clinic.Patients.DisplayStats();
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
                clinic.Doctors.DisplayAll();
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
                clinic.Doctors.Add(doctor);
            }
            else if (choice == "3")
            {
                string query = ReadText("Спеціальність: ");
                Doctor[] found = clinic.Doctors.FindBySpeciality(query);
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
                if (clinic.Doctors.Remove(id.Value))
                    Console.WriteLine("Лікаря видалено.");
                else
                    Console.WriteLine("Лікаря з таким ID не знайдено.");
            }
            else if (choice == "5")
            {
                clinic.Doctors.DisplayStats();
            }
            else
            {
                Console.WriteLine("Невідомий пункт меню.");
            }
        }
    }

    // ===== Підменю «Записи» =====
    static void AppointmentsMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("--- Записи ---");
            Console.WriteLine("1. Показати всі");
            Console.WriteLine("2. Записати пацієнта до лікаря");
            Console.WriteLine("3. Скасувати запис");
            Console.WriteLine("4. Завершити прийом");
            Console.WriteLine("5. Записи пацієнта");
            Console.WriteLine("6. Записи лікаря");
            Console.WriteLine("7. Майбутні записи");
            Console.WriteLine("0. Назад");
            string choice = ReadText("Ваш вибір: ");
            if (inputClosed || choice == "0") return;

            if (choice == "1")
            {
                clinic.Appointments.DisplayList(clinic.Appointments.GetAll());
            }
            else if (choice == "2")
            {
                clinic.Patients.DisplayAll();
                int? patientId = ReadInt("ID пацієнта: ");
                if (patientId == null) continue;
                clinic.Doctors.DisplayAll();
                int? doctorId = ReadInt("ID лікаря: ");
                if (doctorId == null) continue;
                DateTime? date = ReadDate("Дата (дд.мм.рррр): ");
                if (date == null) continue;
                int? hour = ReadInt("Година (0-23): ");
                if (hour == null) continue;
                if (hour.Value < 0 || hour.Value > 23)
                {
                    Console.WriteLine("Година має бути від 0 до 23.");
                    continue;
                }
                int? duration = ReadInt("Тривалість у хвилинах (напр. 30): ");
                if (duration == null) continue;
                clinic.Appointments.Book(patientId.Value, doctorId.Value, date.Value.AddHours(hour.Value), duration.Value);
            }
            else if (choice == "3")
            {
                int? id = ReadInt("ID запису: ");
                if (id == null) continue;
                string reason = ReadText("Причина (можна пропустити): ");
                if (clinic.Appointments.Cancel(id.Value, reason))
                    Console.WriteLine("Запис скасовано.");
                else
                    Console.WriteLine("Неможливо: запис не знайдено або він уже не в стані Scheduled.");
            }
            else if (choice == "4")
            {
                int? id = ReadInt("ID запису: ");
                if (id == null) continue;
                if (clinic.Appointments.Complete(id.Value))
                    Console.WriteLine("Прийом завершено.");
                else
                    Console.WriteLine("Неможливо: запис не знайдено або він уже не в стані Scheduled.");
            }
            else if (choice == "5")
            {
                int? id = ReadInt("ID пацієнта: ");
                if (id == null) continue;
                clinic.Appointments.DisplayList(clinic.Appointments.GetByPatient(id.Value));
            }
            else if (choice == "6")
            {
                int? id = ReadInt("ID лікаря: ");
                if (id == null) continue;
                clinic.Appointments.DisplayList(clinic.Appointments.GetByDoctor(id.Value));
            }
            else if (choice == "7")
            {
                clinic.Appointments.DisplayList(clinic.Appointments.GetUpcoming());
            }
            else
            {
                Console.WriteLine("Невідомий пункт меню.");
            }
        }
    }

    static void ScheduleMenu()
    {
        DateTime? date = ReadDate("Дата (дд.мм.рррр, Enter — завтра): ", true);
        if (date == null) return;
        clinic.DisplaySchedule(date.Value);
    }

    // ===== Задача 8. Зростаючий масив =====
    static void DemoGrowable()
    {
        Console.WriteLine("=== GrowablePatientManager: масив без ліміту ===");
        GrowablePatientManager growable = new GrowablePatientManager();
        Console.WriteLine($"Початкова місткість: {growable.Capacity}");

        for (int i = 1; i <= 20; i++)
        {
            growable.Add(new Patient("Тестовий", "Пацієнт" + i));
        }

        Console.WriteLine($"Додано пацієнтів: {growable.Count}, місткість масиву: {growable.Capacity}");
        Console.WriteLine($"Видалення пацієнта #{growable.GetAll()[0].Id}: {growable.Remove(growable.GetAll()[0].Id)}");
        Console.WriteLine($"Залишилось: {growable.Count}");
        Console.WriteLine("Ліміту немає: масив росте рівно тоді, коли заповнюється.");
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
