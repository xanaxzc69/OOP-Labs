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

    // Робочий графік: година початку і кінця (ціле число від 0 до 23)
    public int WorkStartHour { get; set; }
    public int WorkEndHour { get; set; }

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
        WorkStartHour = 8;
        WorkEndHour = 17;
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

    public int WorkingHoursPerDay => WorkEndHour - WorkStartHour;

    public string WorkSchedule => WorkStartHour.ToString("D2") + ":00–" + WorkEndHour.ToString("D2") + ":00";

    // Чи працює лікар прямо зараз
    public bool IsAvailableNow => DateTime.Now.Hour >= WorkStartHour && DateTime.Now.Hour < WorkEndHour;

    // Чи може лікар прийняти пацієнта у задану годину
    public bool CanAcceptAt(int hour)
    {
        return hour >= WorkStartHour && hour < WorkEndHour;
    }

    public override string ToString()
    {
        string status = IsAvailableNow ? "доступний зараз" : "недоступний зараз";
        return $"[{Id}] {FullName} | {Speciality} | {LicenseNumber} | Тел: {Phone} | " +
               $"{WorkSchedule} ({WorkingHoursPerDay} год) | {status}";
    }
}
