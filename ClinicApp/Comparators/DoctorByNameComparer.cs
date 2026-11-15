using System;
using System.Collections.Generic;
using System.Globalization;
using ClinicApp.Models;

namespace ClinicApp.Comparators;

// Задача 3 (Lab10). Порядок за алфавітом — за зростанням.
//
// Рядки порівнюються з урахуванням мови. Звичайне порівняння за кодами
// символів (StringComparison.Ordinal) для української дало б неправильний
// результат: воно не знає ні про порядок «Г, Ґ, Д», ні про «І, Ї, Й»,
// і великі літери в ньому завжди йдуть раніше за малі.
// Тому беремо правила української культури uk-UA.
public class DoctorByNameComparer : IComparer<DoctorStats>
{
    // Один екземпляр на весь клас: створювати компаратор рядків
    // при кожному порівнянні було б марною роботою
    private static readonly StringComparer NameComparer =
        StringComparer.Create(CultureInfo.GetCultureInfo("uk-UA"), true);

    public int Compare(DoctorStats x, DoctorStats y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        // Зростання: x порівнюється з y у прямому порядку
        int byName = NameComparer.Compare(x.DoctorName, y.DoctorName);
        if (byName != 0) return byName;

        return x.DoctorId.CompareTo(y.DoctorId);
    }
}
