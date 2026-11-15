using System;
using System.Collections.Generic;

namespace ClinicApp.Utils;

// Задача 3 (Lab11). Контейнер результату перевірки.
//
// Валідатор не кидає винятків і нічого не виводить на екран: він лише
// збирає сюди всі знайдені помилки й повертає цей об'єкт. Рішення, що
// робити далі — показати список користувачеві, відхилити додавання
// чи попросити ввести дані ще раз, — ухвалює вже той, хто викликав
// перевірку. Через це один і той самий валідатор однаково придатний
// і для консольного меню, і для автоматичних тестів.
public class ValidationResult
{
    private readonly List<string> _errors = new List<string>();

    // Об'єкт коректний тоді й лише тоді, коли не знайдено жодної помилки
    public bool IsValid => _errors.Count == 0;

    public int ErrorCount => _errors.Count;

    // Копія списку: ззовні додати або прибрати помилку неможливо
    public List<string> Errors => new List<string>(_errors);

    public void AddError(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        _errors.Add(message);
    }

    // Вивід на екран лишається поза валідатором, тому це окремий метод,
    // яким користується меню
    public void Display(string title = "Результат перевірки")
    {
        Console.WriteLine($"=== {title} ===");
        if (IsValid)
        {
            Console.WriteLine("Помилок не знайдено: об'єкт коректний.");
            return;
        }

        Console.WriteLine($"Знайдено помилок: {_errors.Count}");
        for (int i = 0; i < _errors.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {_errors[i]}");
        }
    }

    public override string ToString()
    {
        if (IsValid) return "Об'єкт коректний";
        return $"Помилок: {_errors.Count} — " + string.Join("; ", _errors);
    }
}
