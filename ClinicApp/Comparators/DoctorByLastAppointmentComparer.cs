using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Comparators;

// Задача 3 (Lab10). Порядок за датою останнього прийому: найсвіжіші першими.
//
// Лікарі, у яких ще не було жодного прийому, мають LastAppointmentDate
// рівну DateTime.MinValue, тому за цим правилом вони природно опиняються
// в кінці списку — і це правильно: їм нічого показувати у звіті «хто
// приймав останнім».
public class DoctorByLastAppointmentComparer : IComparer<DoctorStats>
{
    public int Compare(DoctorStats x, DoctorStats y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return 1;
        if (y == null) return -1;

        // Спадання за датою: новіша дата — вище в списку
        int byDate = y.LastAppointmentDate.CompareTo(x.LastAppointmentDate);
        if (byDate != 0) return byDate;

        return x.DoctorId.CompareTo(y.DoctorId);
    }
}
