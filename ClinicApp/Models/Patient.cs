using System;
using ClinicApp.Enums;
using ClinicApp.Utils;

namespace ClinicApp.Models;

// Пацієнт медичної клініки
public class Patient
{
    // Статичний лічильник — один для всіх екземплярів класу
    private static int _nextId = 1;

    // Id лише для читання, присвоюється тільки в конструкторі
    public int Id { get; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }

    // Задача 1: замість string — перерахування BloodType
    public BloodType BloodType { get; set; }

    public string Phone { get; set; }
    public string Email { get; set; } = "";

    // ---------- Конструктори з ланцюжком виклику ----------

    // Повний конструктор — уся ініціалізація тут
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
        // Задача 3: форматування винесено у статичний клас ClinicFormatter
        return $"[{Id}] {FullName} | Вік: {ClinicFormatter.FormatAge(Age)} ({GetAgeCategory()}) | " +
               $"Кров: {ClinicFormatter.FormatBloodType(BloodType)} | Тел: {ClinicFormatter.FormatPhone(Phone)}";
    }
}
