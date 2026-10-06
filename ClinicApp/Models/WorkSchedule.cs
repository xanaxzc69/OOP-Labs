using System;

namespace ClinicApp.Models;

// Робочий графік як value type: дві пов'язані величини об'єднані
// в один нероздільний тип; struct копіюється при присвоєнні.
public struct WorkSchedule
{
    // get-only властивості: після створення графік змінити не можна,
    // можна лише створити новий
    public int Start { get; }
    public int End { get; }

    // Задача 3: конструктор не дозволяє створити безглуздий графік.
    // Ці перевірки унікальні для WorkSchedule, тому лишаються на місці.
    public WorkSchedule(int start, int end)
    {
        if (start < 0 || start > 23)
            throw new ArgumentOutOfRangeException(nameof(start),
                "Година початку роботи має бути від 0 до 23");
        if (end < 1 || end > 24)
            throw new ArgumentOutOfRangeException(nameof(end),
                "Година кінця роботи має бути від 1 до 24");
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
