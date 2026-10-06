using System;

namespace ClinicApp.Models;

// Задача 2. Робочий графік як value type.
// Дві пов'язані величини (початок і кінець) об'єднані в один нероздільний тип:
// struct копіюється при присвоєнні, а не передається за посиланням.
public struct WorkSchedule
{
    // get-only властивості: після створення графік змінити не можна,
    // можна лише створити новий
    public int Start { get; }
    public int End { get; }

    public WorkSchedule(int start, int end)
    {
        if (start < 0 || start > 23)
            throw new ArgumentException("Година початку має бути від 0 до 23");
        if (end < 0 || end > 23)
            throw new ArgumentException("Година кінця має бути від 0 до 23");
        if (start >= end)
            throw new ArgumentException("Початок роботи має бути раніше за кінець");

        Start = start;
        End = end;
    }

    // ---------- Обчислювані властивості ----------

    public int HoursPerDay => End - Start;

    public string Display => Start.ToString("D2") + ":00–" + End.ToString("D2") + ":00";

    // Чи потрапляє поточна година в межі розкладу
    public bool IsNow => Contains(DateTime.Now.Hour);

    // ---------- Методи ----------

    // Година кінця вже не робоча: графік 8–17 охоплює години 8..16
    public bool Contains(int hour)
    {
        return hour >= Start && hour < End;
    }

    public override string ToString()
    {
        return $"{Display} ({HoursPerDay} год)";
    }
}
