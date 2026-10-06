using System;
using ClinicApp.Enums;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Лікар клініки з робочим графіком
public class Doctor
{
    private static int _nextId = 1;

    // Задача 2: приватні поля + явні властивості через них
    private string _firstName;
    private string _lastName;
    private string _licenseNumber;
    private string _phone;

    public int Id { get; }

    public string FirstName
    {
        get { return _firstName; }
        set { _firstName = value; }
    }

    public string LastName
    {
        get { return _lastName; }
        set { _lastName = value; }
    }

    public string LicenseNumber
    {
        get { return _licenseNumber; }
        set { _licenseNumber = value; }
    }

    public string Phone
    {
        get { return _phone; }
        set { _phone = value; }
    }

    // Перерахування Speciality
    public Speciality Speciality { get; set; }

    // Робочий графік — один нероздільний value type (struct WorkSchedule)
    public WorkSchedule Schedule { get; set; }

    // ---------- Конструктори ----------

    // Повний конструктор. Графік за замовчуванням: 8:00–17:00.
    // Значення присвоюються через властивості, а не напряму в поля.
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
        return $"[{Id}] {FullName} | {ClinicFormatter.FormatSpeciality(Speciality)} | {LicenseNumber} | " +
               $"Тел: {ClinicFormatter.FormatPhone(Phone)} | {Schedule.Display} ({WorkingHoursPerDay} год) | {status}";
    }
}
