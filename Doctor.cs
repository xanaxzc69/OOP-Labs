using System;

namespace ClinicApp;

// Лікар клініки з робочим графіком
public class Doctor
{
    private static int _nextId = 1;

    public int Id { get; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    // Задача 1: замість string — перерахування Speciality
    public Speciality Speciality { get; set; }

    public string LicenseNumber { get; set; }
    public string Phone { get; set; }

    // Задача 2: два окремих поля WorkStartHour/WorkEndHour замінено
    // одним нероздільним значенням типу WorkSchedule
    public WorkSchedule Schedule { get; set; }

    // ---------- Конструктори ----------

    // Повний конструктор. Графік за замовчуванням: 8:00–17:00
    public Doctor(string firstName, string lastName, Speciality speciality, string licenseNumber, string phone)
    {
        Id = _nextId++;
        FirstName = firstName;
        LastName = lastName;
        Speciality = speciality;
        LicenseNumber = licenseNumber;
        Phone = phone;
        Schedule = new WorkSchedule(8, 17);
    }

    // Без параметрів -> делегує повному з дефолтними значеннями
    public Doctor() : this("Невідомий", "Лікар", Speciality.Therapist, "Без ліцензії", "0000000000")
    {
    }

    // Ім'я, прізвище, спеціальність -> делегує повному
    public Doctor(string firstName, string lastName, Speciality speciality)
        : this(firstName, lastName, speciality, "Без ліцензії", "0000000000")
    {
    }

    // ---------- Обчислювані властивості ----------

    public string FullName => FirstName + " " + LastName;

    // Робота з графіком делегується структурі WorkSchedule
    public int WorkingHoursPerDay => Schedule.HoursPerDay;

    public bool IsAvailableNow => Schedule.IsNow;

    // Чи може лікар прийняти пацієнта у задану годину
    public bool CanAcceptAt(int hour)
    {
        return Schedule.Contains(hour);
    }

    public override string ToString()
    {
        string status = IsAvailableNow ? "доступний зараз" : "недоступний зараз";
        return $"[{Id}] {FullName} | {Speciality} | {LicenseNumber} | Тел: {Phone} | " +
               $"{Schedule.Display} ({WorkingHoursPerDay} год) | {status}";
    }
}
