using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Comparators;

// Задача 3 (Lab10). Порядок за виручкою: від найбільшої до найменшої.
//
// IComparable<T> вбудований у сам тип і дає лише ОДИН порядок.
// IComparer<T> — окремий клас зі своїм правилом порівняння, і таких
// класів може бути скільки завгодно. Його передають у List<T>.Sort(comparer)
// або Array.Sort(array, comparer), не змінюючи при цьому самого типу.
public class DoctorByRevenueComparer : IComparer<DoctorStats>
{
    public int Compare(DoctorStats x, DoctorStats y)
    {
        // Об'єкти без значення вважаємо «найменшими» й відправляємо в кінець
        if (x == null && y == null) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        // Спадання: порівнюємо y з x, а не навпаки
        int byRevenue = y.TotalRevenue.CompareTo(x.TotalRevenue);
        if (byRevenue != 0) return byRevenue;

        // Однакова виручка — вище той, хто провів більше прийомів
        int byCount = y.AppointmentCount.CompareTo(x.AppointmentCount);
        if (byCount != 0) return byCount;

        return x.DoctorId.CompareTo(y.DoctorId);
    }
}
