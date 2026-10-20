using System;
using ClinicApp.Enums;
using ClinicApp.Interfaces;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Лікар клініки з робочим графіком
// Задача 3: розклад мають саме лікарі, тому контракт ISchedulable
// підписує Doctor, а не Appointment.
public class Doctor : ISchedulable
{
    private static int _nextId = 1;

    // Задача 2: приватні поля + явні властивості через них.
    // Задача 4: повторювані перевірки виконує ClinicValidator
    private string _firstName;
    private string _lastName;
    private string _licenseNumber;
    private string _phone;

    public int Id { get; }

    public string FirstName
    {
        get { return _firstName; }
        set
        {
            ClinicValidator.ValidateName(value, "Ім'я");
            _firstName = value.Trim();
        }
    }

    public string LastName
    {
        get { return _lastName; }
        set
        {
            ClinicValidator.ValidateName(value, "Прізвище");
            _lastName = value.Trim();
        }
    }

    public string LicenseNumber
    {
        get { return _licenseNumber; }
        set
        {
            // Правило зустрічається лише один раз, тому лишається на місці
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Номер ліцензії не може бути порожнім");
            _licenseNumber = value.Trim();
        }
    }

    public string Phone
    {
        get { return _phone; }
        set
        {
            ClinicValidator.ValidatePhone(value);
            _phone = value;
        }
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

    // ---------- Реалізація контракту ISchedulable ----------

    // Чи потрапляє вказаний момент часу в робочий графік
    public bool CanSchedule(DateTime at)
    {
        return Schedule.Contains(at.Hour);
    }

    // Цілогодинні слоти на вказаний день, починаючи з початку розкладу.
    // Їх не більше за slotCount і не більше, ніж годин у розкладі.
    public DateTime[] GetAvailableSlots(DateTime date, int slotCount)
    {
        ClinicValidator.ValidatePositive(slotCount, "Кількість слотів");

        int count = slotCount;
        if (count > Schedule.HoursPerDay)
        {
            count = Schedule.HoursPerDay;
        }

        DateTime[] slots = new DateTime[count];
        for (int i = 0; i < count; i++)
        {
            slots[i] = date.Date.AddHours(Schedule.Start + i);
        }
        return slots;
    }

    public override string ToString()
    {
        string status = IsAvailableNow ? "доступний зараз" : "недоступний зараз";
        return $"[{Id}] {FullName} | {ClinicFormatter.FormatSpeciality(Speciality)} | {LicenseNumber} | " +
               $"Тел: {ClinicFormatter.FormatPhone(Phone)} | {Schedule.Display} ({WorkingHoursPerDay} год) | {status}";
    }
}
