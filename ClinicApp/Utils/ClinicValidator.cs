using System;
using System.Text.RegularExpressions;

namespace ClinicApp.Utils;

// Задача 4. Правила, які повторюються в кількох класах, винесені сюди:
// перевірка імені лежала у чотирьох сеттерах, перевірка телефону — у двох.
// Тепер правило описане в одному місці, а сеттери лише викликають потрібний метод.
// Клас статичний: у нього немає стану і створювати його екземпляри немає сенсу.
public static class ClinicValidator
{
    public const int MaxNameLength = 50;
    public const int PhoneLength = 10;
    public const int MinYear = 1900;

    // Задача 5. Регулярні вирази створюються один раз (static readonly),
    // а не на кожному виклику методу — це помітно швидше.
    private static readonly Regex PhoneRegex = new Regex(@"^\d{10}$");
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$");

    // Ім'я або прізвище: не порожнє, не з самих пробілів, не довше 50 символів
    public static void ValidateName(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} не може бути порожнім або складатися з пробілів");
        if (value.Trim().Length > MaxNameLength)
            throw new ArgumentException($"{fieldName} не може бути довшим за {MaxNameLength} символів");
    }

    // Телефон: рівно 10 цифр. Замість циклу по символах — один Regex.IsMatch
    public static void ValidatePhone(string phone)
    {
        if (phone == null || !PhoneRegex.IsMatch(phone))
            throw new ArgumentException($"Телефон має містити рівно {PhoneLength} цифр");
    }

    // Email: базовий формат «щось@щось.домен».
    // Порожній рядок означає «email невідомий» і перевіряється окремо в Patient.
    public static void ValidateEmail(string email)
    {
        if (email == null || !EmailRegex.IsMatch(email))
            throw new ArgumentException("Некоректний формат email (очікується вигляд name@example.com)");
    }

    // Дата: не в майбутньому і не раніше 1900 року
    public static void ValidateDate(DateTime date, string fieldName)
    {
        if (date.Date > DateTime.Today)
            throw new ArgumentOutOfRangeException(fieldName, $"{fieldName}: дата не може бути в майбутньому");
        if (date.Year < MinYear)
            throw new ArgumentOutOfRangeException(fieldName, $"{fieldName}: дата не може бути раніше {MinYear} року");
    }

    // Додатне число: більше за 0
    public static void ValidatePositive(int value, string fieldName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(fieldName, $"{fieldName} має бути більшим за 0");
    }
}
