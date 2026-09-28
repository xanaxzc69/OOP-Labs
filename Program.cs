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
        SeedPatients();
        SeedDoctors();
        SeedAppointments();
        DemoEnums();
        DemoWorkSchedule();
        DemoFormatter();
        MainMenu();

        Console.WriteLine("До побачення!");
    }

    // ===== Початкові дані =====
    static void SeedPatients()
    {
        // 5 пацієнтів, створених різними конструкторами
        Patient p1 = new Patient();                                      // без параметрів
        Patient p2 = new Patient("Іван", "Петренко");                    // (firstName, lastName)
        Patient p3 = new Patient("Олена", "Коваль", new DateTime(1985, 3, 14), BloodType.APositive, "0671234567");
        Patient p4 = new Patient("Марія", "Шевченко", new DateTime(2015, 11, 2), BloodType.OPositive, "0509876543");
        Patient p5 = new Patient("Петро", "Бондар", new DateTime(1955, 7, 20), BloodType.BNegative, "0931112233");
        p3.Email = "olena.koval@gmail.com";

        clinic.Patients.Add(p1);
        clinic.Patients.Add(p2);
        clinic.Patients.Add(p3);
        clinic.Patients.Add(p4);
        clinic.Patients.Add(p5);
        clinic.Patients.DisplayAll();
        Console.WriteLine();
    }

    static void SeedDoctors()
    {
        Doctor d1 = new Doctor("Сергій", "Литвин", Speciality.Therapist, "LIC-1001", "0671112233");
        Doctor d2 = new Doctor("Ірина", "Ткач", Speciality.Cardiologist, "LIC-1002", "0502223344");
        Doctor d3 = new Doctor("Андрій", "Гнатюк", Speciality.Surgeon);
        Doctor d4 = new Doctor();

        // Задача 2: графік — одне значення типу WorkSchedule
        d2.Schedule = new WorkSchedule(10, 18);
        d3.Schedule = new WorkSchedule(14, 22);

        clinic.Doctors.Add(d1);
        clinic.Doctors.Add(d2);
        clinic.Doctors.Add(d3);
        clinic.Doctors.Add(d4);
        clinic.Doctors.DisplayAll();
        Console.WriteLine();
    }

    static void SeedAppointments()
    {
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
    }

    // ===== Задача 1. Enum замість магічних рядків =====
    static void DemoEnums()
    {
        Console.WriteLine("=== Задача 1. Enum замість рядків ===");
        Appointment[] all = clinic.Appointments.GetAll();
        Console.WriteLine($"Статус першого запису: {all[0].Status}");
        Console.WriteLine($"all[0].Status == AppointmentStatus.Scheduled -> {all[0].Status == AppointmentStatus.Scheduled}");

        Console.Write("Усі стани запису:");
        foreach (AppointmentStatus status in Enum.GetValues(typeof(AppointmentStatus)))
        {
            Console.Write(" " + status);
        }
        Console.WriteLine();

        Console.WriteLine($"Значень у BloodType: {Enum.GetValues(typeof(BloodType)).Length}");
        Console.WriteLine($"Значень у Speciality: {Enum.GetValues(typeof(Speciality)).Length}");
        Console.WriteLine("Одруківку на кшталт \"Sheduled\" тепер не написати — не скомпілюється.");
        Console.WriteLine();
    }


    // ===== Задача 2. struct WorkSchedule =====
    static void DemoWorkSchedule()
    {
        Console.WriteLine("=== Задача 2. struct WorkSchedule (value type) ===");
        Doctor doctor = clinic.Doctors.GetAll()[0];

        WorkSchedule copy = doctor.Schedule;            // структура копіюється при присвоєнні
        doctor.Schedule = new WorkSchedule(7, 15);      // міняємо графік лікаря

        Console.WriteLine($"Графік лікаря тепер: {doctor.Schedule.Display}");
        Console.WriteLine($"Зроблена раніше копія не змінилась: {copy.Display}");
        Console.WriteLine($"Годин на день: {doctor.Schedule.HoursPerDay}, працює зараз: {doctor.Schedule.IsNow}");
        Console.WriteLine($"Contains(8): {doctor.Schedule.Contains(8)}, Contains(20): {doctor.Schedule.Contains(20)}");
        Console.WriteLine($"ToString(): {doctor.Schedule}");

        try
        {
            WorkSchedule nonsense = new WorkSchedule(20, 6);
            Console.WriteLine(nonsense.Display);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Графік 20–6 відхилено: {ex.Message}");
        }
        Console.WriteLine();
    }


    // ===== Задача 3. Статичний клас та індексатори =====
    static void DemoFormatter()
    {
        Console.WriteLine("=== Задача 3. ClinicFormatter та індексатор ===");
        Console.WriteLine($"{BloodType.APositive} -> {ClinicFormatter.FormatBloodType(BloodType.APositive)}");
        Console.WriteLine($"{BloodType.BNegative} -> {ClinicFormatter.FormatBloodType(BloodType.BNegative)}");
        Console.WriteLine($"{Speciality.Cardiologist} -> {ClinicFormatter.FormatSpeciality(Speciality.Cardiologist)}");
        Console.WriteLine($"Вік: {ClinicFormatter.FormatAge(41)}, {ClinicFormatter.FormatAge(33)}, " +
                          $"{ClinicFormatter.FormatAge(16)}, {ClinicFormatter.FormatAge(11)}");
        Console.WriteLine($"Телефон: 0501234567 -> {ClinicFormatter.FormatPhone("0501234567")}");

        // Індексатор: замість FindById(3) — просто [2]
        Console.WriteLine($"clinic.Patients[2]: {clinic.Patients[2]}");
        Console.WriteLine($"clinic.Doctors[1]: {clinic.Doctors[1]}");
        Console.WriteLine($"clinic.Appointments[0]: {clinic.Appointments[0]}");
        Console.WriteLine();
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
            Console.WriteLine("0. Вихід");
            string choice = ReadText("Ваш вибір: ");
            if (inputClosed || choice == "0") return;

            if (choice == "1") PatientsMenu();
            else if (choice == "2") DoctorsMenu();
            else if (choice == "3") AppointmentsMenu();
            else if (choice == "4") ScheduleMenu();
            else if (choice == "5") clinic.GenerateReport();
            else Console.WriteLine("Невідомий пункт меню.");
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
            Console.WriteLine("6. Пацієнт за номером у списку (індексатор)");
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
                BloodType? bloodType = ReadEnum<BloodType>("Оберіть групу крові:");
                if (bloodType == null) continue;
                string phone = ReadText("Телефон: ");
                if (firstName.Length == 0 || lastName.Length == 0)
                {
                    Console.WriteLine("Ім'я та прізвище не можуть бути порожніми.");
                    continue;
                }
                clinic.Patients.Add(new Patient(firstName, lastName, dob.Value, bloodType.Value, phone));
            }
            else if (choice == "3")
            {
                string query = ReadText("Введіть ім'я або прізвище: ");
                Patient[] found = clinic.Patients.FindByName(query);
                if (found.Length == 0) Console.WriteLine("Нікого не знайдено.");
                for (int i = 0; i < found.Length; i++) Console.WriteLine(found[i]);
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
            else if (choice == "6")
            {
                int? index = ReadInt($"Номер у списку (0 - {clinic.Patients.Count - 1}): ");
                if (index == null) continue;
                Patient patient = clinic.Patients[index.Value];
                Console.WriteLine(patient != null ? patient.ToString() : "Пацієнта з таким номером немає.");
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
            Console.WriteLine("3. Знайти за спеціальністю (текст)");
            Console.WriteLine("4. Видалити");
            Console.WriteLine("5. Статистика");
            Console.WriteLine("6. Лікар за номером у списку (індексатор)");
            Console.WriteLine("0. Назад");
            string choice = ReadText("Ваш вибір: ");
            if (inputClosed || choice == "0") return;

            if (choice == "1")
            {
                clinic.Doctors.DisplayAll();
            }
            else if (choice == "2")
            {
                AddDoctorFromConsole();
            }
            else if (choice == "3")
            {
                string query = ReadText("Спеціальність (частина назви): ");
                Doctor[] found = clinic.Doctors.FindBySpeciality(query);
                if (found.Length == 0) Console.WriteLine("Лікарів не знайдено.");
                for (int i = 0; i < found.Length; i++) Console.WriteLine(found[i]);
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
            else if (choice == "6")
            {
                int? index = ReadInt($"Номер у списку (0 - {clinic.Doctors.Count - 1}): ");
                if (index == null) continue;
                Doctor doctor = clinic.Doctors[index.Value];
                Console.WriteLine(doctor != null ? doctor.ToString() : "Лікаря з таким номером немає.");
            }
            else
            {
                Console.WriteLine("Невідомий пункт меню.");
            }
        }
    }

    static void AddDoctorFromConsole()
    {
        string firstName = ReadText("Ім'я: ");
        string lastName = ReadText("Прізвище: ");
        Speciality? speciality = ReadEnum<Speciality>("Оберіть спеціальність:");
        if (speciality == null) return;
        string license = ReadText("Номер ліцензії: ");
        string phone = ReadText("Телефон: ");
        int? start = ReadInt("Година початку роботи (0-23): ");
        if (start == null) return;
        int? end = ReadInt("Година кінця роботи (0-23): ");
        if (end == null) return;

        Doctor doctor = new Doctor(firstName, lastName, speciality.Value, license, phone);

        try
        {
            doctor.Schedule = new WorkSchedule(start.Value, end.Value);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Некоректний графік: " + ex.Message);
            return;
        }
        clinic.Doctors.Add(doctor);
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
                    Console.WriteLine("Неможливо: запис не знайдено або він уже не Scheduled.");
            }
            else if (choice == "4")
            {
                int? id = ReadInt("ID запису: ");
                if (id == null) continue;
                if (clinic.Appointments.Complete(id.Value))
                    Console.WriteLine("Прийом завершено.");
                else
                    Console.WriteLine("Неможливо: запис не знайдено або він уже не Scheduled.");
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

    // Вибір значення перерахування зі списку
    static T? ReadEnum<T>(string title) where T : struct, Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        Console.WriteLine(title);
        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {values[i]}");
        }
        int? choice = ReadInt("Номер: ");
        if (choice == null) return null;
        if (choice.Value < 1 || choice.Value > values.Length)
        {
            Console.WriteLine("Некоректний номер.");
            return null;
        }
        return values[choice.Value - 1];
    }
}
