using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Comparators;

// Задача 3 (Lab10). Порядок пацієнтів за сумою витрат: від найбільшої.
//
// Той самий тип PatientStats уже має природний порядок «найактивніший
// перший» (за кількістю візитів). Цей компаратор дає клініці другий
// погляд на ті самі дані — за грошима, — і для цього не треба було
// змінювати ані сам клас PatientStats, ані його CompareTo.
public class PatientBySpendingComparer : IComparer<PatientStats>
{
    public int Compare(PatientStats x, PatientStats y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        // Спадання за сумою витрат
        int bySpent = y.TotalSpent.CompareTo(x.TotalSpent);
        if (bySpent != 0) return bySpent;

        // Однакова сума — вище той, хто приходив частіше
        int byVisits = y.VisitCount.CompareTo(x.VisitCount);
        if (byVisits != 0) return byVisits;

        return x.PatientId.CompareTo(y.PatientId);
    }
}
