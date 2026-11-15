using System;

namespace ClinicApp.Models;

// Задача 1 (Lab10). Аналітичний знімок по лікарю.
//
// Це незмінний (immutable) об'єкт: усі властивості мають лише get і
// присвоюються один раз у конструкторі. Знімок описує стан на момент
// обчислення, тому змінювати його після створення не має сенсу —
// інакше звіт міг би «поїхати» вже після того, як його сформували.
//
// IComparable<DoctorStats> задає природний порядок типу: порядок
// «за замовчуванням», яким скористаються Array.Sort() і List.Sort()
// без жодних додаткових параметрів. Для аналітики природним є
// навантаження: чим більше прийомів провів лікар, тим вище він у списку.
public class DoctorStats : IComparable<DoctorStats>
{
    public int DoctorId { get; }
    public string DoctorName { get; }
    public int AppointmentCount { get; }
    public decimal TotalRevenue { get; }
    public DateTime LastAppointmentDate { get; }

    public DoctorStats(int doctorId, string doctorName, int appointmentCount,
                       decimal totalRevenue, DateTime lastAppointmentDate)
    {
        DoctorId = doctorId;
        DoctorName = doctorName;
        AppointmentCount = appointmentCount;
        TotalRevenue = totalRevenue;
        LastAppointmentDate = lastAppointmentDate;
    }

    // Чи був у лікаря хоч один прийом. Якщо жодного — дати останнього
    // прийому не існує, і замість неї зберігається DateTime.MinValue.
    public bool HasAppointments => AppointmentCount > 0;

    // ---------- Природний порядок ----------

    // CompareTo повертає:
    //   від'ємне число — поточний об'єкт має стояти ПЕРЕД other,
    //   нуль           — порядок між ними не визначений,
    //   додатне число  — поточний об'єкт має стояти ПІСЛЯ other.
    //
    // Нам потрібне спадання за кількістю прийомів, тому порівняння
    // розвернуте: other порівнюється з this, а не навпаки.
    public int CompareTo(DoctorStats other)
    {
        // null вважаємо «найменшим», тому будь-який реальний об'єкт стоїть раніше
        if (other == null) return -1;

        int byCount = other.AppointmentCount.CompareTo(AppointmentCount);
        if (byCount != 0) return byCount;

        // Однакове навантаження — вище той, хто приніс більше виручки
        int byRevenue = other.TotalRevenue.CompareTo(TotalRevenue);
        if (byRevenue != 0) return byRevenue;

        // Остання, стабілізуюча ознака: за Id, щоб порядок був однозначним
        return DoctorId.CompareTo(other.DoctorId);
    }

    public override string ToString()
    {
        string lastDate = HasAppointments
            ? LastAppointmentDate.ToString("dd.MM.yyyy")
            : "—";

        return $"[{DoctorId}] {DoctorName} | прийомів: {AppointmentCount} | " +
               $"виручка: {TotalRevenue} грн | останній прийом: {lastDate}";
    }
}
