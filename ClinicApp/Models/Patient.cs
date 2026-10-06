using System;
using ClinicApp.Enums;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Пацієнт медичної клініки
public class Patient
{
    // Статичний лічильник — один для всіх екземплярів класу
    private static int _nextId = 1;

    // Задача 2: дані зберігаються у приватних полях, доступ — через властивості.
    // Задача 3: сеттери перевіряють значення і кидають виняток,
    // тому поле фізично не може отримати некоректне значення.
    private string _firstName;
    private string _lastName;
    private DateTime _dateOfBirth;
    private string _phone;

    // Id лише для читання, присвоюється тільки в конструкторі
    public int Id { get; }

    public string FirstName
    {
        get { return _firstName; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Ім'я не може бути порожнім або складатися з пробілів");
            if (value.Trim().Length > 50)
                throw new ArgumentException($"Ім'я не може бути довшим за 50 символів");
            _firstName = value.Trim();
        }
    }

    public string LastName
    {
        get { return _lastName; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Прізвище не може бути порожнім або складатися з пробілів");
            if (value.Trim().Length > 50)
                throw new ArgumentException($"Прізвище не може бути довшим за 50 символів");
            _lastName = value.Trim();
        }
    }

    public DateTime DateOfBirth
    {
        get { return _dateOfBirth; }
        set
        {
            if (value.Date > DateTime.Today)
                throw new ArgumentOutOfRangeException(nameof(DateOfBirth),
                    "Дата народження не може бути в майбутньому");
            if (value.Year < 1900)
                throw new ArgumentOutOfRangeException(nameof(DateOfBirth),
                    "Дата народження не може бути раніше 1900 року");
            _dateOfBirth = value;
        }
    }

    public string Phone
    {
        get { return _phone; }
        set
        {
            if (value == null || value.Length != 10)
                throw new ArgumentException("Телефон має містити рівно 10 символів");
            foreach (char symbol in value)
            {
                if (!char.IsDigit(symbol))
                    throw new ArgumentException("Телефон має складатися лише з цифр");
            }
            _phone = value;
        }
    }

    // Перерахування BloodType — некоректне значення неможливе за типом
    public BloodType BloodType { get; set; }

    public string Email { get; set; } = "";

    // ---------- Конструктори з ланцюжком виклику ----------

    // Повний конструктор — уся ініціалізація тут.
    // Значення присвоюються через властивості, а не напряму в поля.
    public Patient(string firstName, string lastName, DateTime dateOfBirth, BloodType bloodType, string phone)
    {
        Id = _nextId++;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        BloodType = bloodType;
        Phone = phone;
    }

    // Без параметрів -> делегує конструктору (firstName, lastName)
    public Patient() : this("Невідомий", "Пацієнт")
    {
    }

    // Ім'я та прізвище -> делегує повному конструктору зі значеннями за замовчуванням
    public Patient(string firstName, string lastName)
        : this(firstName, lastName, new DateTime(2000, 1, 1), BloodType.Unknown, "0000000000")
    {
    }

    // ---------- Обчислювані властивості ----------

    public string FullName => FirstName + " " + LastName;

    // Вік з урахуванням того, чи вже був день народження цього року
    public int Age
    {
        get
        {
            DateTime today = DateTime.Today;
            int age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age))
            {
                age--;   // день народження цього року ще не настав
            }
            return age;
        }
    }

    public bool IsAdult => Age >= 18;

    // ---------- Метод класифікації ----------

    public string GetAgeCategory()
    {
        if (Age < 18) return "дитина";
        if (Age < 60) return "дорослий";
        return "літній";
    }

    public override string ToString()
    {
        // Форматування винесено у статичний клас ClinicFormatter
        return $"[{Id}] {FullName} | Вік: {ClinicFormatter.FormatAge(Age)} ({GetAgeCategory()}) | " +
               $"Кров: {ClinicFormatter.FormatBloodType(BloodType)} | Тел: {ClinicFormatter.FormatPhone(Phone)}";
    }
}
